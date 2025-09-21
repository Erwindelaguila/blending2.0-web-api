using System.Security.Claims;
using Function.Blending.Upload.Functions.Configuration.Options;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace Function.Blending.Upload.Functions.Support.Security.PrincipalBuilders;

public sealed class DevBypassPrincipalBuilder : IPrincipalBuilder
{
  private readonly AuthorizationOptions _opts;
  public DevBypassPrincipalBuilder(IOptions<AuthorizationOptions> opts) => _opts = opts.Value;

  public Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req)
  {
    // No DevBypass para webhooks
    var isWebhook = ctx.Items.TryGetValue("IsWebhook", out var v) && v is bool b && b;
    if (!_opts.DevBypass || isWebhook)
      return Task.FromResult<ClaimsPrincipal?>(null);

    var claims = _opts.DevGroups.Select(g => new Claim("groups", g)).ToList();
    claims.Add(new Claim("dev", "true"));

    var identity = new ClaimsIdentity(claims, "DevBypass");
    return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
  }
}
