using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FwxHttp = Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Function.Blending.Opt.Shared.Security;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class BearerPrincipalBuilder : IPrincipalBuilder
{
  private readonly IConfiguration _cfg;
  private readonly ITokenValidationService _tokenValidator;

  public BearerPrincipalBuilder(IConfiguration cfg, ITokenValidationService tokenValidator)
  {
    _cfg = cfg;
    _tokenValidator = tokenValidator;
  }

  public async Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, FwxHttp.HttpRequestData req)
  {
    if (!bool.TryParse(_cfg["Auth:EnableBearerTokens"], out var enabled) || !enabled)
      return null;

    if (!req.Headers.TryGetValues("Authorization", out var authVals))
      return null;

    var auth = authVals.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      return null;

    var jwtRaw = auth["Bearer ".Length..].Trim();
    var mode = _cfg["Auth:Bearer:ValidationMode"] ?? "Strict";

    if (mode.Equals("Relaxed", StringComparison.OrdinalIgnoreCase))
      return BuildPrincipalRelaxed(jwtRaw);

    // Strict → servicio
    return await _tokenValidator.ValidateAndNormalizeAsync(jwtRaw);
  }

  private ClaimsPrincipal? BuildPrincipalRelaxed(string jwtRaw)
  {
    try
    {
      var handler = new JwtSecurityTokenHandler();
      var jwt = handler.ReadJwtToken(jwtRaw); // NO valida firma

      var opts = new JwtClaimsFactory.Options
      {
        AuthType = "Bearer-Relaxed",
        AuthModeTag = "relaxed",
        ValidateLifetime = bool.TryParse(_cfg["Auth:Bearer:ValidateLifetime"], out var vl) && vl,
        IncludeGroups = true,
        IncludeWids = true,
        IncludeRoles = true,
        IncludeScope = true,
      };

      return JwtClaimsFactory.CreatePrincipal(jwt, opts);
    }
    catch
    {
      return null;
    }
  }
}
