using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
               
        builder.ToTable("Producto");
        
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Producto_Activo_Nombre_COVERING")
               .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => new { e.CalidadId, e.Activo }, "IX_Producto_CalidadId_Activo");

        builder.HasIndex(e => new { e.CalidadId, e.TipoProduccionId, e.Activo }, "IX_Producto_Calidad_TipoProduccion_Activo");

        builder.HasIndex(e => new { e.CreadoEl, e.ModificadoEl }, "IX_Producto_CreadoEl_ModificadoEl")
               .IsDescending();

        builder.HasIndex(e => new { e.TipoProduccionId, e.Activo }, "IX_Producto_TipoProduccionId_Activo");

        builder.HasIndex(e => e.Codigo, "UQ_Producto_Codigo")
               .IsUnique();
        
        builder.Property(e => e.Id)
               .HasDefaultValueSql("(newid())");

        builder.Property(e => e.Activo)
               .HasDefaultValue(true);

        builder.Property(e => e.Codigo)
               .HasMaxLength(20)
               .IsUnicode(false);

        builder.Property(e => e.CreadoEl)
               .HasDefaultValueSql("(sysdatetime())");

        builder.Property(e => e.Descripcion)
               .HasMaxLength(150)
               .IsUnicode(false);

        builder.Property(e => e.Nombre)
               .HasMaxLength(50)
               .IsUnicode(false);
        
        builder.HasOne(d => d.Calidad)
               .WithMany(p => p.Productos)
               .HasForeignKey(d => d.CalidadId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Producto_CalidadId");

        builder.HasOne(d => d.TipoProduccion)
               .WithMany(p => p.Productos)
               .HasForeignKey(d => d.TipoProduccionId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Producto_TipoProduccionId");
    }
}