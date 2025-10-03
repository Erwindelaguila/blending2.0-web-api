using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Persistence.Support;
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class LogEjecucionOutputRepository(
  IDbContextFactory<BlendingDbContext> dbFactory,
  IMapper mapper
) : PooledQueryRepository<BlendingDbContext>(dbFactory), ILogEjecucionOutputRepository
{
  public Task<IReadOnlyList<LogOutContenedor>> GetContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.LogOutContenedor>()
                        .AsNoTracking()
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<LogOutContenedor>>(list);
  }, ct);

  public Task<IReadOnlyList<LogOutContenedor>> GetDeepContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.LogOutContenedor>()
                        .AsNoTracking()
                        .Include(x => x.LogOutConDistribucion)
                        .Include(x => x.LogOutConComposicion)
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<LogOutContenedor>>(list);
  }, ct);
}
