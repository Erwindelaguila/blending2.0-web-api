using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Function.Blending.Core.Infrastructure.Persistence.Configurations;

public class CalidadParametroConfiguration: IEntityTypeConfiguration<CalidadParametro>
{
    public void Configure(EntityTypeBuilder<CalidadParametro> builder)
    {
        builder.ToTable("CalidadParametro");
        
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.CalidadId, e.Activo }, "IX_CalidadParametro_CalidadId_Activo");

        builder.HasIndex(e => new { e.CalidadId, e.ParametroId, e.Activo }, "IX_CalidadParametro_Calidad_Parametro_COVERING")
            .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => new { e.CalidadId, e.ParametroId }, "IX_CalidadParametro_Calidad_Parametro_UNIQUE")
            .IsUnique()
            .HasFilter("([Activo]=(1))");

        builder.HasIndex(e => new { e.ParametroId, e.Activo }, "IX_CalidadParametro_ParametroId_Activo");

        builder.HasIndex(e => new { e.Valor, e.Activo }, "IX_CalidadParametro_Valor_Activo");
        
        builder.Property(e => e.Id)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEl)
            .HasDefaultValueSql("(sysdatetime())");

        builder.Property(e => e.Valor)
            .HasColumnType("decimal(10, 4)");

        // 🔗 Relaciones
        builder.HasOne(d => d.Calidad)
            .WithMany(p => p.CalidadParametros)
            .HasForeignKey(d => d.CalidadId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CalidadParametro_CalidadId");

        builder.HasOne(d => d.Parametro)
            .WithMany(p => p.CalidadParametros)
            .HasForeignKey(d => d.ParametroId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CalidadParametro_ParametroId");
    }
}