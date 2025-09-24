using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models
using RM = Function.Blending.Opt.Domain.ReadModels;                 // ReadModels

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class CalInpParameterRepository(BlendingDbContext db, IMapper mapper): ICalInpParameterRepository
{
  public async Task<IReadOnlyList<RM.CalInpParametroItemRm>> GetParametersByEjecucionIdAsync(Guid ejecucionId, CancellationToken ct)
  {
    var list = await db.Set<EF.CalInpParametro>()
                        .AsNoTracking()
                        .Include(x => x.Calidad)
                        .Include(x => x.Parametro)
                        .Where(x => x.EjecucionId == ejecucionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<RM.CalInpParametroItemRm>>(list);
  }
}
