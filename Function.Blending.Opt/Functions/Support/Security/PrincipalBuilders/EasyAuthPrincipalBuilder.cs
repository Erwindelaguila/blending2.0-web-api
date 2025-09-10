using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class EasyAuthPrincipalBuilder : IPrincipalBuilder
{
  private readonly IConfiguration _cfg;
  public EasyAuthPrincipalBuilder(IConfiguration cfg) => _cfg = cfg;

  public Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req)
  {
    if (!bool.TryParse(_cfg["Auth:EnableEasyAuth"], out var enabled) || !enabled)
      return Task.FromResult<ClaimsPrincipal?>(null);

    if (!req.Headers.TryGetValues("X-MS-CLIENT-PRINCIPAL", out var vals))
      return Task.FromResult<ClaimsPrincipal?>(null);

    try
    {
      var raw = vals.FirstOrDefault();
      if (string.IsNullOrWhiteSpace(raw))
        return Task.FromResult<ClaimsPrincipal?>(null);

      var json = Encoding.UTF8.GetString(Convert.FromBase64String(raw));
      var dto = JsonSerializer.Deserialize<ClientPrincipalDto>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      var claims = dto?.Claims?.Select(c => new Claim(c.Type, c.Value)).ToList() ?? new List<Claim>();
      var identity = new ClaimsIdentity(claims, "EasyAuth");
      return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(identity));
    }
    catch
    {
      return Task.FromResult<ClaimsPrincipal?>(null);
    }
  }

  private sealed record ClientPrincipalDto(string IdentityProvider, string UserId, string UserDetails, IEnumerable<ClientClaim> Claims);
  private sealed record ClientClaim(string Type, string Value) { public ClientClaim() : this("", "") { } }
}
