using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FwxHttp = Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;

public sealed class BearerPrincipalBuilder(IConfiguration cfg, ITokenValidationService tokenValidator) : IPrincipalBuilder
{
  public async Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, FwxHttp.HttpRequestData req)
  {
    if (!bool.TryParse(cfg[ConfigurationKeys.Auth.EnableBearer], out var enabled) || !enabled)
      return null;

    if (!req.Headers.TryGetValues(AuthConstants.Authorization, out var authVals))
      return null;

    var auth = authVals.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith($"{AuthConstants.BearerAuth.Name} ", StringComparison.OrdinalIgnoreCase))
      return null;

    var jwtRaw = auth[$"{AuthConstants.BearerAuth.Name} ".Length..].Trim();
    var mode = cfg[ConfigurationKeys.Auth.Bearer.ValidationMode] ?? AuthConstants.BearerAuth.Modes.Strict;

    if (mode.Equals(AuthConstants.BearerAuth.Modes.Relaxed, StringComparison.OrdinalIgnoreCase))
      return BuildPrincipalRelaxed(jwtRaw);

    // Strict → servicio
    return await tokenValidator.ValidateAndNormalizeAsync(jwtRaw);
  }

  private ClaimsPrincipal? BuildPrincipalRelaxed(string jwtRaw)
  {
    try
    {
      var handler = new JwtSecurityTokenHandler();
      var jwt = handler.ReadJwtToken(jwtRaw); // NO valida firma

      var opts = new JwtClaimsFactory.Options
      {
        AuthType = AuthConstants.BearerAuth.Types.Relaxed,
        AuthModeTag = AuthConstants.BearerAuth.Tags.Relaxed,
        ValidateLifetime = bool.TryParse(cfg[ConfigurationKeys.Auth.Bearer.ValidateLifetime], out var vl) && vl,
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
