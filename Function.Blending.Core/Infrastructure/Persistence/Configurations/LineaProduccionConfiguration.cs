using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class LineaProduccionConfiguration: IEntityTypeConfiguration<LineaProduccion>
{
    public void Configure(EntityTypeBuilder<LineaProduccion> builder)
    {
        builder.ToTable("LineaProduccion");

        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.Activo, e.Codigo }, "IX_LineaProduccion_Activo_Codigo")
            .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => e.CreadoEl, "IX_LineaProduccion_CreadoEl_DESC")
            .IsDescending();

        builder.HasIndex(e => e.Codigo, "UQ_LineaProduccion_Codigo")
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