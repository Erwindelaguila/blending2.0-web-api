using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class PlantaConfiguration : IEntityTypeConfiguration<Plantum>
{
    public void Configure(EntityTypeBuilder<Plantum> builder)
    {
        // Tabla
        builder.ToTable("Planta");

        // Clave primaria
        builder.HasKey(e => e.Id);

        // Índices
        builder.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Planta_Activo_Nombre")
               .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => new { e.CreadoEl, e.CreadoPorId }, "IX_Planta_CreadoEl_CreadoPorId")
               .IsDescending(true, false);

        builder.HasIndex(e => new { e.NumeroRuma, e.Activo }, "IX_Planta_NumeroRuma_Activo");

        builder.HasIndex(e => e.Codigo, "UQ_Planta_Codigo")
               .IsUnique();

        // Propiedades
        builder.Property(e => e.Id)
               .HasDefaultValueSql("(newid())");

        builder.Property(e => e.Codigo)
               .IsRequired()
               .HasMaxLength(20)
               .IsUnicode(false);

        builder.Property(e => e.Nombre)
               .IsRequired()
               .HasMaxLength(50)
               .IsUnicode(false);

        builder.Property(e => e.Descripcion)
               .HasMaxLength(150)
               .IsUnicode(false);

        builder.Property(e => e.NumeroRuma)
               .IsRequired();

        builder.Property(e => e.Activo)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(e => e.CreadoPorId)
               .IsRequired();

        builder.Property(e => e.CreadoEl)
               .HasDefaultValueSql("(sysdatetime())")
               .HasColumnType("datetime")
               .IsRequired();

        builder.Property(e => e.ModificadoPorId);

        builder.Property(e => e.ModificadoEl)
               .HasColumnType("datetime");
    }
}
