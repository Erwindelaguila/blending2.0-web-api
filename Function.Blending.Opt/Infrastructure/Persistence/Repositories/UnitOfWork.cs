using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Application.Abstractions;

namespace Function.Blending.Opt.Infrastructure.Persistence;

[Obsolete("No se usa. Transacciones locales en repos. Mantener solo como referencia.")]
public sealed class UnitOfWork(BlendingDbContext db) : IUnitOfWork
{
  public Task<int> SaveChangesAsync(CancellationToken ct = default)
      => db.SaveChangesAsync(ct);
}
