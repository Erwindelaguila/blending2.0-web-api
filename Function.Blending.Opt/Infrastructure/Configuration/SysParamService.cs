using System;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;

namespace Function.Blending.Opt.Infrastructure.Configuration
{
  /// <summary>Capa de servicio minimal: puede crecer (métricas, validaciones, tracing) sin tocar EF.</summary>
  public sealed class SysParamService : ISysParamService
  {
    private readonly ISysParamRepository _repo;

    public SysParamService(ISysParamRepository repo)
    {
      _repo = repo;
    }

    public Task<Guid?> GetIdAsync(string sysParamKey, CancellationToken ct)
      => _repo.GetIdAsync(sysParamKey, ct);

    public async Task<Guid> GetRequiredIdAsync(string sysParamKey, CancellationToken ct)
    {
      var id = await _repo.GetIdAsync(sysParamKey, ct);
      if (id is null)
        throw new InvalidOperationException($"SysParam key '{sysParamKey}' not found or inactive.");
      return id.Value;
    }
  }
}
