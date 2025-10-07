using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Persistence.Support;
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models
using VO = Function.Blending.Opt.Domain.ValueObjects;               // ValueObjects

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class LogEjecucionInputRepository(
  IDbContextFactory<BlendingDbContext> dbFactory,
  IMapper mapper
) : PooledQueryRepository<BlendingDbContext>(dbFactory), ILogEjecucionInputRepository
{
  public Task<LogInpInfo> GetInfoByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var info = await db.Set<EF.LogInpInfo>()
                         .AsNoTracking()
                         .Where(x => x.EjecucionId == executionId)
                         .FirstOrDefaultAsync(ct);
    return mapper.Map<VO.LogInpInfo>(info);
  }, ct);

  public Task<LogInpFiltro> GetFiltroByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var filtro = await db.Set<EF.LogInpFiltro>()
                       .AsNoTracking()
                       .Include(x => x.LogInpFilCapacidad)
                       .Include(x => x.LogInpFilDivision)
                       .Include(x => x.LogInpFilEmparejamiento)
                       .Where(x => x.EjecucionId == executionId)
                       .FirstOrDefaultAsync(ct);
    return mapper.Map<LogInpFiltro>(filtro);
  }, ct);

  public Task<LogInpDemanda> GetDemandaByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var demanda = await db.Set<EF.LogInpDemanda>()
                       .AsNoTracking()
                       .Include(x => x.LogInpDemParametro)
                       .Where(x => x.EjecucionId == executionId)
                       .FirstOrDefaultAsync(ct);
    return mapper.Map<LogInpDemanda>(demanda);
  }, ct);

  public Task<IReadOnlyList<LogInpOferta>> GetOfertaByExecutionIdAsync(Guid executionId, CancellationToken ct) => WithDbAsync(async db =>
  {
    var oferta = await db.Set<EF.LogInpOferta>()
                       .AsNoTracking()
                       .Include(x => x.LogInpOfeParametro)
                       .Include(x => x.LogInpOfeOtros)
                       .Where(x => x.EjecucionId == executionId)
                       .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<LogInpOferta>>(oferta);
  }, ct);
}
