using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void RegisterDomainRepositories(IServiceCollection services)
  {
    services.AddScoped<ICalEjecucionRepository, CalEjecucionRepository>();
    services.AddScoped<ILogEjecucionRepository, LogEjecucionRepository>();
    services.AddScoped<ICalEjecucionInputRepository, CalEjecucionInputRepository>();
    services.AddScoped<ICalEjecucionOutputRepository, CalEjecucionOutputRepository>();
    services.AddScoped<ILogEjecucionInputRepository, LogEjecucionInputRepository>();
    services.AddScoped<ILogEjecucionOutputRepository, LogEjecucionOutputRepository>();
  }
}
