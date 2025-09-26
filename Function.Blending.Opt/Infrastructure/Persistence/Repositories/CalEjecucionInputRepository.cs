using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Persistence.Support;
using Microsoft.EntityFrameworkCore;
// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models
using RM = Function.Blending.Opt.Domain.ReadModels;                 // ReadModels
using VO = Function.Blending.Opt.Domain.ValueObjects;               // ValueObjects

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class CalEjecucionInputRepository(
  IDbContextFactory<BlendingDbContext> dbFactory,
  IMapper mapper
) : PooledQueryRepository<BlendingDbContext>(dbFactory), ICalEjecucionInputRepository
{
  public Task<VO.CalInpFiltro> GetFilterByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
    {
      var filter = await db.Set<EF.CalInpFiltro>()
                           .AsNoTracking()
                           .Where(x => x.EjecucionId == executionId)
                           .FirstOrDefaultAsync(ct);
      return mapper.Map<VO.CalInpFiltro>(filter);
    }, ct);

  public Task<IReadOnlyList<RM.CalInpParametroItemRm>> GetParametersItemRmByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
    {
      var list = await db.Set<EF.CalInpParametro>()
                         .AsNoTracking()
                         .Include(x => x.Calidad)
                         .Include(x => x.Parametro)
                         .Where(x => x.EjecucionId == executionId)
                         .ToListAsync(ct);
      return mapper.Map<IReadOnlyList<RM.CalInpParametroItemRm>>(list);
    }, ct);
}
