using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class TipoProduccionConfiguration : IEntityTypeConfiguration<TipoProduccion>
{
    public void Configure(EntityTypeBuilder<TipoProduccion> builder)
    {
        builder.ToTable("TipoProduccion");
        
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.AgregadoId, e.Activo }, "IX_TipoProduccion_AgregadoId_Activo");

        builder.HasIndex(e => new { e.CreadoEl, e.Activo }, "IX_TipoProduccion_CreadoEl_Activo")
               .IsDescending(true, false);

        builder.HasIndex(e => new { e.LineaProduccionId, e.Activo }, "IX_TipoProduccion_LineaProduccionId_Activo");

        builder.HasIndex(e => new { e.LineaProduccionId, e.AgregadoId, e.Activo }, "IX_TipoProduccion_LineaProduccion_Agregado_Activo");

        builder.HasIndex(e => e.Codigo, "UQ_TipoProduccion_Codigo")
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
        
        builder.HasOne(d => d.Agregado)
               .WithMany(p => p.TipoProduccions)
               .HasForeignKey(d => d.AgregadoId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TipoProduccion_AgregadoId");

        builder.HasOne(d => d.LineaProduccion)
               .WithMany(p => p.TipoProduccions)
               .HasForeignKey(d => d.LineaProduccionId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TipoProduccion_LineaProduccionId");
    }
}
