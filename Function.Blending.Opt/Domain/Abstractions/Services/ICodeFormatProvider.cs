using System.Threading;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ICodeFormatProvider
{
  /// <summary>Formato para el código final de EJECUCIÓN (Calidad).</summary>
  Task<string> GetQualityExecutionFormatAsync(CancellationToken ct);
  /// <summary>Formato para el código final de EJECUCIÓN (Logística).</summary>
  Task<string> GetLogisticExecutionFormatAsync(CancellationToken ct);
}
