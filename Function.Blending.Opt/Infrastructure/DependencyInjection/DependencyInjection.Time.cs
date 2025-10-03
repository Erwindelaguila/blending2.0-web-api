using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Services.Time;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Options.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureTime(IServiceCollection services, IConfiguration cfg)
  {
    services.AddSingleton<ITimeZoneService, TimeZoneService>();

    // ===== Options (binding plano) =====
    services.AddOptions<TimeZoneOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        var rawEnf = config[ConfigurationKeys.Time.EnforceValidTimeZone];
        var rawTim = config[ConfigurationKeys.Time.TimeZoneId];
        var rawWin = config[ConfigurationKeys.Time.WindowsTimeZoneId];
        var rawIan = config[ConfigurationKeys.Time.IanaTimeZoneId];

        opts.EnforceValidTimeZone = bool.TryParse(rawEnf, out var g) && g;
        opts.TimeZoneId = string.IsNullOrWhiteSpace(rawTim) ? rawTim : default;
        opts.WindowsTimeZoneId = string.IsNullOrWhiteSpace(rawWin) ? rawWin : default;
        opts.IanaTimeZoneId = string.IsNullOrWhiteSpace(rawIan) ? rawIan : default;
      });
  }
}
