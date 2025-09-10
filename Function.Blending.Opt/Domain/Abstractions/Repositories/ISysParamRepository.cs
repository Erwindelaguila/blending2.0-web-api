using System;
using System.Threading;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories
{
  public interface ISysParamRepository
  {
    /// <summary>Obtiene el Guid (Id) de un SysParam por su Key lógica (solo activos).</summary>
    Task<Guid?> GetIdAsync(string key, CancellationToken ct);
  }
}
