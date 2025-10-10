using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Persistence;

public partial class BlendingDbContext
{
  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    // Aplica TODAS las IEntityTypeConfiguration<> del ensamblado
    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(BlendingDbContext).Assembly
    );
  }
}
