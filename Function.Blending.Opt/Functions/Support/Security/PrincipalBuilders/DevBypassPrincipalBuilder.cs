using Function.Blending.Opt.Functions.Configuration.Options;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class DevBypassPrincipalBuilder : IPrincipalBuilder
{
  private readonly AuthorizationOptions _opts;
  public DevBypassPrincipalBuilder(IOptions<AuthorizationOptions> opts) => _opts = opts.Value;

  public Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req)
  {
    // No DevBypass para webhooks
    var isWebhook = ctx.Items.TryGetValue(MiscellaneousConstants.IsWebhook, out var v) && v is bool b && b;
    if (!_opts.DevBypass || isWebhook)
      return Task.FromResult<ClaimsPrincipal?>(null);

    var claims = _opts.DevGroups.Select(g => new Claim(AuthConstants.Groups, g)).ToList();
    claims.Add(new Claim(AuthConstants.DevBypassAuth.Dev, "true"));

    var identity = new ClaimsIdentity(claims, AuthConstants.DevBypassAuth.Name);
    return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
  }
}
