using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class ParametroConfiguration : IEntityTypeConfiguration<Parametro>
{
    public void Configure(EntityTypeBuilder<Parametro> builder)
    {
        builder.ToTable("Parametro");
        
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Parametro_Activo_Nombre_COVERING")
            .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => new { e.CreadoEl, e.ModificadoEl }, "IX_Parametro_CreadoEl_ModificadoEl")
            .IsDescending();

        builder.HasIndex(e => e.Codigo, "UQ_Parametro_Codigo")
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
    }
}