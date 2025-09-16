using System;
using Function.Blending.Opt.Infrastructure.Persistence;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void RegisterDbContext(IServiceCollection services, IConfiguration cfg)
  {
    services.AddDbContext<BlendingDbContext>((sp, opt) =>
    {
      var cs = cfg.GetConnectionString(ConfigurationKeys.ConnectionStrings.BlendingDbName) ?? cfg[ConfigurationKeys.ConnectionStrings.BlendingDb] ?? cfg[ConfigurationKeys.ConnectionStrings.SqlDb];

      if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException(
          $"Falta la cadena de conexión '{ConfigurationKeys.ConnectionStrings.BlendingDbName}' " +
          $"(clave '{ConfigurationKeys.ConnectionStrings.BlendingDb}').");

      opt.UseSqlServer(cs);
    });
  }
}
