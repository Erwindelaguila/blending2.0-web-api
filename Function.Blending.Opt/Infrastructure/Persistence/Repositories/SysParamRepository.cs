using System;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories
{
  /// <summary>Repositorio EF sin caché.</summary>
  public sealed class SysParamRepository(BlendingDbContext db) : ISysParamRepository
  {

    public async Task<Guid?> GetIdAsync(string key, CancellationToken ct)
    {
      if (string.IsNullOrWhiteSpace(key))
        return null;

      var rec = await db.Set<SysParam>()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(p => p.Key == key && p.IsActive, ct);

      return rec?.Id;
    }
  }
}
