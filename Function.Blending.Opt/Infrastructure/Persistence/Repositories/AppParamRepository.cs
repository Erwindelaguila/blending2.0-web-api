using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Persistence;
using Function.Blending.Opt.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación EF Core para lectura de parámetros (AppParam).
/// </summary>
public sealed class AppParamRepository(BlendingDbContext db) : IAppParamRepository
{
  public async Task<string?> GetValueAsync(string key, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(key)) return null;

    // Clave exacta y activo
    var rec = await db.Set<AppParam>()
                      .AsNoTracking()
                      .FirstOrDefaultAsync(p => p.Key == key && p.IsActive, ct);

    return rec?.Value;
  }
}
