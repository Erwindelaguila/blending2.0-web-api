using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class CalidadConfiguration : IEntityTypeConfiguration<Calidad>
{
    public void Configure(EntityTypeBuilder<Calidad> builder)
    {
        {
            builder.ToTable("Calidad");
            
            builder.HasKey(e => e.Id);
            
            builder.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Calidad_Activo_Nombre_COVERING")
                .HasFilter("([Activo]=(1))");

            builder.HasIndex(e => new { e.CodigoMaterial, e.Activo }, "IX_Calidad_CodigoMaterial_Activo")
                .HasFilter("([CodigoMaterial] IS NOT NULL)");

            builder.HasIndex(e => new { e.NoConforme, e.Activo }, "IX_Calidad_NoConforme_Activo");

            builder.HasIndex(e => e.Codigo, "UQ_Calidad_Codigo")
                .IsUnique();
            
            builder.Property(e => e.Id)
                .HasDefaultValueSql("(newid())");

            builder.Property(e => e.Activo)
                .HasDefaultValue(true);

            builder.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(e => e.CodigoMaterial)
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
        }
    }
}