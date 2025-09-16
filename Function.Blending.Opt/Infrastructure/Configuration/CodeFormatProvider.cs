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
    var fromDb = await appParams.GetValueAsync(cfg[ConfigurationKeys.AppParam.Keys.Quality.ExecutionCodeFormat] ?? AppParamDefaults.Keys.QualityExecutionFormat, ct);
    if (!string.IsNullOrWhiteSpace(fromDb))
      return fromDb!;

    // 2) Fallbacks por configuración (Default preferido; Legacy por retro-compat)
    return cfg[ConfigurationKeys.Defaults.Quality.ExecutionFormat] ?? AppParamDefaults.Values.QualityExecutionFormat;
  }
  public async Task<string> GetLogisticExecutionFormatAsync(CancellationToken ct)
  {
    // 1) AppParam (cacheado por tu decorator)
    var fromDb = await appParams.GetValueAsync(cfg[ConfigurationKeys.AppParam.Keys.Logistics.ExecutionCodeFormat] ?? AppParamDefaults.Keys.LogisticExecutionFormat, ct);
    if (!string.IsNullOrWhiteSpace(fromDb))
      return fromDb!;

    // 2) Fallbacks por configuración (Default preferido; Legacy por retro-compat)
    return cfg[ConfigurationKeys.Defaults.Logistic.ExecutionFormat] ?? AppParamDefaults.Values.LogisticExecutionFormat;
  }
}
