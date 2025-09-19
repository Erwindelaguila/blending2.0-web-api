using Function.Blending.Opt.Application.Abstractions.External;
using Function.Blending.Opt.Infrastructure.Configuration.Options.External;
using Function.Blending.Opt.Infrastructure.Services.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureExternalClients(IServiceCollection services, IConfiguration cfg)
  {
    var baseUrl = cfg["External_CalidadModel_BaseUrl"] ?? string.Empty;
    var startPath = cfg["External_CalidadModel_StartPath"] ?? "/api/quality/start";
    var timeoutSeconds = int.TryParse(cfg["External_CalidadModel_TimeoutSeconds"], out var t) ? Math.Max(1, t) : 30;
    var apiKey = cfg["External_CalidadModel_ApiKey"];

    var options = new CalidadModelOptions
    {
      BaseUrl = baseUrl,
      StartPath = startPath,
      TimeoutSeconds = timeoutSeconds,
      ApiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey
    };

    services.AddSingleton<IOptions<CalidadModelOptions>>(Options.Create(options));

    services.AddHttpClient<ICalidadModelStarter, CalidadModelStarterHttp>((sp, http) =>
    {
      var opts = sp.GetRequiredService<IOptions<CalidadModelOptions>>().Value;
      if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
        http.BaseAddress = new Uri(opts.BaseUrl, UriKind.Absolute);

      http.Timeout = TimeSpan.FromSeconds(Math.Max(1, opts.TimeoutSeconds));
    });
  }
}
