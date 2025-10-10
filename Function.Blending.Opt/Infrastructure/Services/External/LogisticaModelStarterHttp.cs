using Function.Blending.Opt.Application.Abstractions.External;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;
using Function.Blending.Opt.Infrastructure.Configuration.Options.External;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Function.Blending.Opt.Infrastructure.Services.External;

public sealed class LogisticaModelStarterHttp : ILogisticaModelStarter
{
  private readonly HttpClient _http;
  private readonly ILogger<LogisticaModelStarterHttp> _logger;
  private readonly IOptions<LogisticaModelOptions> _opts;

  public LogisticaModelStarterHttp(
    HttpClient http,
    IOptions<LogisticaModelOptions> opts,
    ILogger<LogisticaModelStarterHttp> logger)
  {
    _http = http;
    _opts = opts;
    _logger = logger;

    var baseUrl = opts.Value.BaseUrl;
    if (string.IsNullOrWhiteSpace(_http.BaseAddress?.ToString()) && !string.IsNullOrWhiteSpace(baseUrl))
      _http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);

    _http.Timeout = TimeSpan.FromSeconds(Math.Max(1, opts.Value.TimeoutSeconds));
  }

  public Task<DispatchResult> StartAsync(LogisticaModelPayload payload, CancellationToken ct)
  {
    var options = _opts.Value;
    var url = new Uri(_http.BaseAddress!, options.StartPath);

    var json = JsonSerializer.Serialize(payload);

    var req = new HttpRequestMessage(HttpMethod.Post, url)
    {
      Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    // evitar 100-continue (handshake extra)
    req.Headers.ExpectContinue = false;

    if (!string.IsNullOrWhiteSpace(options.ApiKey))
      req.Headers.Add("x-api-key", options.ApiKey);

    // ======= FIRE-AND-FORGET REAL: no esperamos headers ni status =======
    var sendTask = _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);  // CancellationToken.None: para NO atar al request original

    // registrar errores sin reventar el request
    _ = sendTask.ContinueWith(t =>
    {
      try
      {
        if (t.IsFaulted)
          _logger.LogError(t.Exception, "Error al enviar start al modelo externo (fire-and-forget).");

        if (t.IsCompletedSuccessfully)
        {
          // liberar recursos de la respuesta si llegó
          t.Result.Dispose();
        }
      }
      finally
      {
        // liberar la request después del envío
        req.Dispose();
      }
    }, TaskScheduler.Default);

    // devolvemos inmediatamente; no nos importa el ACK ni el cuerpo
    return Task.FromResult(DispatchResult.Ok());
  }
}
