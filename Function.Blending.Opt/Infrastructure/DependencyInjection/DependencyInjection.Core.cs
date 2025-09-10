using Function.Blending.Opt.Application.Abstractions;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration;
using Function.Blending.Opt.Infrastructure.Time;
using Function.Blending.Opt.Shared.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void RegisterCoreInfra(IServiceCollection services)
  {
    services.AddMemoryCache();             // IMemoryCache
    services.AddSingleton<IClock, SystemClock>();
  }

  private static void RegisterTimeAndCodes(IServiceCollection services)
  {
    services.AddSingleton<IExecutionCodeGenerator, CalExecutionCodeGenerator>();
    services.AddScoped<ICodeFormatProvider, CodeFormatProvider>();
  }
}
