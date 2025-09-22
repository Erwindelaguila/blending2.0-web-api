using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Shared.Constants;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class HmacPrincipalBuilder : IPrincipalBuilder
{
  private readonly IConfiguration _cfg;
  private readonly IFunctionAttributeReader _attrReader;

  public HmacPrincipalBuilder(IConfiguration cfg, IFunctionAttributeReader attrReader)
  {
    _cfg = cfg;
    _attrReader = attrReader;
  }

  public Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req)
  {
    // Feature flag (controlas activación sin redeploy)
    if (!bool.TryParse(_cfg[ConfigurationKeys.Auth.EnableHmacPrincipal], out var enabled) || !enabled)
      return Task.FromResult<ClaimsPrincipal?>(null);

    // Requiere HMAC válido (marcado por el middleware)
    var hmacOk = ctx.Items.TryGetValue(AuthConstants.HmacAuth.ItemsKey, out var hv) && hv is bool b && b;
    if (!hmacOk)
      return Task.FromResult<ClaimsPrincipal?>(null);

    // Necesitamos saber qué scope exige la función
    var scopesAttr = _attrReader.Get<RequireScopesAttribute>(ctx);
    var firstScope = scopesAttr?.Scopes?.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(firstScope))
      return Task.FromResult<ClaimsPrincipal?>(null);

    var hmacGroupsKey = firstScope.Replace(AuthConstants.AllowPrefix, AuthConstants.HmacAuth.HmacGroupsPrefix);
    var csv = _cfg[hmacGroupsKey] ?? string.Empty;

    var groups = csv.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (groups.Length == 0)
      return Task.FromResult<ClaimsPrincipal?>(null);

    var claims = groups.Select(g => new Claim(AuthConstants.Groups, g)).ToList();
    claims.Add(new Claim(AuthConstants.AuthType, AuthConstants.HmacAuth.Name));

    var identity = new ClaimsIdentity(claims, AuthConstants.HmacAuth.Name);
    return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
  }
}
