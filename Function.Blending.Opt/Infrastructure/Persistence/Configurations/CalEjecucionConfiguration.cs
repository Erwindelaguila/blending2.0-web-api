using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Configurations;

public class CalEjecucionConfiguration : IEntityTypeConfiguration<CalEjecucion>
{
  public void Configure(EntityTypeBuilder<CalEjecucion> builder)
  {
    builder
        .HasOne(le => le.Estado)
        .WithMany()
        .HasForeignKey(le => le.EstadoId)
        .OnDelete(DeleteBehavior.NoAction);

    builder.HasIndex(le => le.EstadoId).HasDatabaseName("IX_CalEjecucion_EstadoId");
  }
}
