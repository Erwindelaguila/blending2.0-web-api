using Function.Blending.Opt.Application.Abstractions.External;
using Function.Blending.Opt.Infrastructure.Configuration.Options.External;
using Function.Blending.Opt.Infrastructure.Services.External;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureExternalClients(IServiceCollection services, IConfiguration cfg)
  {
    var baseUrl = cfg[ConfigurationKeys.ExternalApi.QualityModel.BaseUrl] ?? string.Empty;
    var startPath = cfg[ConfigurationKeys.ExternalApi.QualityModel.StartPath] ?? "/api/quality/start";
    var timeoutSeconds = int.TryParse(cfg[ConfigurationKeys.ExternalApi.QualityModel.TimeoutSeconds], out var t) ? Math.Max(1, t) : 30;
    var apiKey = cfg[ConfigurationKeys.ExternalApi.QualityModel.ApiKey];

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
