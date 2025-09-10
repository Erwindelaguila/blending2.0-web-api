using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure.Configuration;

// Primary constructor
public sealed class CodeFormatProvider(
  IAppParamRepository appParams,
  IConfiguration cfg
) : ICodeFormatProvider
{
  public async Task<string> GetQualityExecutionFormatAsync(CancellationToken ct)
  {
    // 1) AppParam (cacheado por tu decorator)
    var fromDb = await appParams.GetValueAsync(cfg[AppParamConstants.Keys.Quality.ExecutionCodeFormat] ?? "APP_CAL_CODIGO_FORMAT", ct);
    if (!string.IsNullOrWhiteSpace(fromDb))
      return fromDb!;

    // 2) Fallbacks por configuración (Default preferido; Legacy por retro-compat)
    return cfg[ConfigurationKeys.Defaults.Quality.ExecutionFormat] ?? "CAL{0:D6}";
  }
  public async Task<string> GetLogisticExecutionFormatAsync(CancellationToken ct)
  {
    // 1) AppParam (cacheado por tu decorator)
    var fromDb = await appParams.GetValueAsync(cfg[AppParamConstants.Keys.Logistics.ExecutionCodeFormat] ?? "APP_LOG_CODIGO_FORMAT", ct);
    if (!string.IsNullOrWhiteSpace(fromDb))
      return fromDb!;

    // 2) Fallbacks por configuración (Default preferido; Legacy por retro-compat)
    return cfg[ConfigurationKeys.Defaults.Logistic.ExecutionFormat] ?? "LOG{0:D6}";
  }
}
