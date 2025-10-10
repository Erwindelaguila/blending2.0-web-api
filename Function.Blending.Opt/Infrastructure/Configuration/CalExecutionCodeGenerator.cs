using System;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure.Configuration;

public sealed class CalExecutionCodeGenerator() : IExecutionCodeGenerator
{
  public string MakeTemp() => "TMP" + Guid.NewGuid().ToString("N")[..17];

  public string MakeFinal(string format, long secuencial) => string.Format(format, secuencial);
}
