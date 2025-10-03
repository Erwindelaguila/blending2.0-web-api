using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Persistence.Support;
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models
using VO = Function.Blending.Opt.Domain.ValueObjects;               // Value objects

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class CalEjecucionOutputRepository(
  IDbContextFactory<BlendingDbContext> dbFactory,
  IMapper mapper
) : PooledQueryRepository<BlendingDbContext>(dbFactory), ICalEjecucionOutputRepository
{
  public Task<IReadOnlyList<VO.CalOutResumen>> GetSummariesByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.CalOutResumen>()
                        .AsNoTracking()
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<VO.CalOutResumen>>(list);
  }, ct);

  public Task<IReadOnlyList<VO.CalOutResumen>> GetDeepSummariesByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.CalOutResumen>()
                        .AsNoTracking()
                        .Include(x => x.CalOutResParametro)
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<VO.CalOutResumen>>(list);
  }, ct);

  public Task<IReadOnlyList<VO.CalOutDetalle>> GetDetailsByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.CalOutDetalle>()
                        .AsNoTracking()
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<VO.CalOutDetalle>>(list);
  }, ct);

  public Task<IReadOnlyList<VO.CalOutDetalle>> GetDeepDetailsByExcecutionAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var list = await db.Set<EF.CalOutDetalle>()
                        .AsNoTracking()
                        .Include(x => x.CalOutDetParametro)
                        .Include(x => x.CalOutDetOtros)
                        .Where(x => x.EjecucionId == executionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<VO.CalOutDetalle>>(list);
  }, ct);
}
