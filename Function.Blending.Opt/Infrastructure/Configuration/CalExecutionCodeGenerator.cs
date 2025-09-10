using System;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure.Configuration;

public sealed class CalExecutionCodeGenerator(IConfiguration cfg) : IExecutionCodeGenerator
{
  private readonly string _defaultFormat = cfg[ConfigurationKeys.Defaults.Quality.ExecutionFormat] ?? "CAL{0:D6}";

  public string MakeTemp() => "TMP" + Guid.NewGuid().ToString("N")[..17];

  public string MakeFinal(long secuencial) => string.Format(_defaultFormat, secuencial);

  public string MakeFinal(string format, long secuencial) => string.Format(string.IsNullOrWhiteSpace(format) ? _defaultFormat : format, secuencial);
}
