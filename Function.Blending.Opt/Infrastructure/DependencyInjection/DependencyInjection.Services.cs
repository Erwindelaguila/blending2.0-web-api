using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Services;
using Function.Blending.Opt.Infrastructure.Services.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void RegisterDomainServices(IServiceCollection services)
  {
    services.AddScoped<ICalOutResumenService, CalOutResumenService>();
  }
}
