using System.Net;
using Function.Blending.Upload.Functions.Support.Authorization;
using Function.Blending.Upload.Functions.Support.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Upload.Functions.Triggers.Health;

public sealed class HealthCheckFunction
{
    private static readonly DateTimeOffset _startedAt = DateTimeOffset.UtcNow;

    [Function(nameof(HealthCheckFunction))]
    [AllowAnonymous]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "head", Route = FunctionRoutes.Health.Check)]
        HttpRequestData req)
    {
        // HEAD -> sólo status
        if (string.Equals(req.Method, "HEAD", StringComparison.OrdinalIgnoreCase))
            return req.CreateResponse(HttpStatusCode.OK);

        var res = req.CreateResponse(HttpStatusCode.OK);

        var body = new
        {
            status = "Healthy",
            utcNow = DateTimeOffset.UtcNow,
            uptimeSeconds = (long)(DateTimeOffset.UtcNow - _startedAt).TotalSeconds,
            version = typeof(HealthCheckFunction).Assembly.GetName().Version?.ToString() ?? "0.0.0"
        };

        await res.WriteAsJsonAsync(body); // minimalista (usa System.Text.Json por defecto)
        return res;
    }
}