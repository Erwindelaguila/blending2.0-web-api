using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Function.Blending.Opt.Functions.Support.Authorization;

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
    if (!bool.TryParse(_cfg["Auth_EnableHmacPrincipal"], out var enabled) || !enabled)
      return Task.FromResult<ClaimsPrincipal?>(null);

    // Requiere HMAC válido (marcado por el middleware)
    var hmacOk = ctx.Items.TryGetValue("HmacValid", out var hv) && hv is bool b && b;
    if (!hmacOk)
      return Task.FromResult<ClaimsPrincipal?>(null);

    // Necesitamos saber qué scope exige la función
    var scopesAttr = _attrReader.Get<RequireScopesAttribute>(ctx);
    var firstScope = scopesAttr?.Scopes?.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(firstScope))
      return Task.FromResult<ClaimsPrincipal?>(null);

    var hmacGroupsKey = firstScope.Replace("Auth_Allow_", "Auth_Hmac_Groups_");
    var csv = _cfg[hmacGroupsKey] ?? string.Empty;

    var groups = csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (groups.Length == 0)
      return Task.FromResult<ClaimsPrincipal?>(null);

    var claims = groups.Select(g => new Claim("groups", g)).ToList();
    claims.Add(new Claim("auth_type", "hmac"));

    var identity = new ClaimsIdentity(claims, "Hmac");
    return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
  }
}
