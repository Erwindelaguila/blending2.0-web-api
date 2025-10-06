using System.Reflection;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Function.Blending.Opt.Functions.Triggers.Health;

public sealed class HealthCheckFunction
{
  private static readonly DateTimeOffset _startedAt = DateTimeOffset.UtcNow;

  [Function(nameof(HealthCheckFunction))]
  [AllowAnonymous]
  public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", "head", Route = FunctionRoutes.Health.Check)]
      HttpRequestData req)
  {
    // HEAD -> solo status
    if (string.Equals(req.Method, "HEAD", StringComparison.OrdinalIgnoreCase))
      return req.CreateResponse(HttpStatusCode.OK);

    var res = req.CreateResponse(HttpStatusCode.OK);

    // Básico siempre
    var assembly = Assembly.GetExecutingAssembly();
    var ver = assembly.GetName().Version?.ToString() ?? "0.0.0";
    var infoVer = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

    var body = new Dictionary<string, object?>
    {
      ["status"] = "Healthy",
      ["utcNow"] = DateTimeOffset.UtcNow,
      ["uptimeSeconds"] = (long)(DateTimeOffset.UtcNow - _startedAt).TotalSeconds,
      ["version"] = ver,
      ["informationalVersion"] = infoVer
    };

    // Solo si se habilita por variable
    if (bool.TryParse(Environment.GetEnvironmentVariable("HEALTHCHECK_INCLUDE_DEPLOY_INFO"), out var include) && include)
    {
      body["deploy"] = new
      {
        deployedAt = Environment.GetEnvironmentVariable("DEPLOYED_AT"),
        buildVersion = Environment.GetEnvironmentVariable("BUILD_VERSION"),
        commit = Environment.GetEnvironmentVariable("GIT_SHA"),
        pipelineId = Environment.GetEnvironmentVariable("PIPELINE_ID"),
        pipelineUrl = Environment.GetEnvironmentVariable("PIPELINE_URL"),
        branch = Environment.GetEnvironmentVariable("BRANCH"),
        environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
      };
    }

    await res.WriteAsJsonAsync(body);
    return res;
  }
}
