using System.Threading;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

/// <summary>
/// Acceso de solo-lectura a parámetros de aplicación (AppParam).
/// </summary>
public interface IAppParamRepository
{
  /// <summary>
  /// Obtiene el valor de un parámetro activo por su clave. Retorna null si no existe o no está activo.
  /// </summary>
  Task<string?> GetValueAsync(string key, CancellationToken ct);
}
