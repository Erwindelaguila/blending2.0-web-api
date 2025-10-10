using Function.Blending.Opt.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class LocalHeaderPrincipalBuilder : IPrincipalBuilder
{
  private readonly IConfiguration _cfg;
  public LocalHeaderPrincipalBuilder(IConfiguration cfg) => _cfg = cfg;

  public Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req)
  {
    if (!bool.TryParse(_cfg[ConfigurationKeys.Auth.EnableLocalHeaderPrincipal], out var enabled) || !enabled)
      return Task.FromResult<ClaimsPrincipal?>(null);

    if (!req.Headers.TryGetValues(AuthConstants.LocalHeaderAuth.HeaderKey, out var localGroupsVals))
      return Task.FromResult<ClaimsPrincipal?>(null);

    var csv = localGroupsVals.FirstOrDefault() ?? string.Empty;
    var groups = csv.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (groups.Length == 0)
      return Task.FromResult<ClaimsPrincipal?>(null);

    var claims = groups.Select(g => new Claim(AuthConstants.Groups, g)).ToList();
    var identity = new ClaimsIdentity(claims, AuthConstants.LocalHeaderAuth.Name);
    return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
  }
}
