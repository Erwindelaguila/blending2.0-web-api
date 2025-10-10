using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories;
using Function.Blending.Opt.Infrastructure.Services.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  /// <summary>
  /// Registro de componentes de logging hacia DDBB (dbo.SysLog).
  /// Lee configuración plana (Values) con prefijo: Logging_Db_*.
  /// </summary>
  private static void ConfigureSysLog(IServiceCollection services, IConfiguration cfg)
  {
    // ===== Logging a DDBB =====
    services.AddScoped<ISysLogRepository, SysLogRepository>();
    services.AddScoped<ISysLogService, SysLogService>();
    services.AddSingleton(SysLogOptions.From(cfg)); // <-- lee claves planas Logging_Db_*
  }
}
