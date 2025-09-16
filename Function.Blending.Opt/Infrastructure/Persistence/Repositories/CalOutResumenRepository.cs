using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models; // EF models
using VO = Function.Blending.Opt.Domain.ValueObjects;               // Value objects


namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class CalOutResumenRepository(BlendingDbContext db, IMapper mapper) : ICalOutResumenRepository
{
  public async Task<IReadOnlyList<VO.CalOutResumen>> GetAllByExcecutionAsync(Guid ejecucionId, CancellationToken ct)
  {
    var list = await db.Set<EF.CalOutResumen>()
                        .AsNoTracking()
                        .Where(x => x.EjecucionId == ejecucionId)
                        .ToListAsync(ct);
    return mapper.Map<IReadOnlyList<VO.CalOutResumen>>(list);
  }
}
