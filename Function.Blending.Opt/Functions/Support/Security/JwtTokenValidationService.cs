using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Function.Blending.Opt.Functions.Support.Security;

public sealed class JwtTokenValidationService : ITokenValidationService
{
  private readonly IConfiguration _cfg;

  private static readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _oidcManagers = new();

  public JwtTokenValidationService(IConfiguration cfg) => _cfg = cfg;

  public async Task<ClaimsPrincipal?> ValidateAndNormalizeAsync(string jwtRaw)
  {
    try
    {
      var tenantId = _cfg["Auth_Bearer_TenantId"];
      var authority = _cfg["Auth_Bearer_Authority"];
      if (string.IsNullOrWhiteSpace(authority) && !string.IsNullOrWhiteSpace(tenantId))
        authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

      if (string.IsNullOrWhiteSpace(authority))
        return null;

      var audienceCsv = _cfg["Auth_Bearer_Audience"] ?? string.Empty;
      var audiences = audienceCsv
        .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToArray();

      if (audiences.Length == 0)
        return null;

      var validIssuerOverride = _cfg["Auth:Bearer:ValidIssuer"];
      var clockSkewSec = int.TryParse(_cfg["Auth_Bearer_ClockSkewSeconds"], out var cs) ? cs : 300;

      var manager = _oidcManagers.GetOrAdd(authority, auth =>
        new ConfigurationManager<OpenIdConnectConfiguration>(
          $"{auth}/.well-known/openid-configuration",
          new OpenIdConnectConfigurationRetriever(),
          new HttpDocumentRetriever { RequireHttps = auth.StartsWith("https://", StringComparison.OrdinalIgnoreCase) }
        )
      );

      var oidc = await manager.GetConfigurationAsync(CancellationToken.None);

      var tvp = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = string.IsNullOrWhiteSpace(validIssuerOverride)
            ? $"https://login.microsoftonline.com/{tenantId}/v2.0"
            : validIssuerOverride,

        ValidateAudience = true,
        ValidAudiences = audiences,

        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = oidc.SigningKeys,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(clockSkewSec),
      };

      var handler = new JwtSecurityTokenHandler();
      var principal = handler.ValidateToken(jwtRaw, tvp, out var validatedToken);

      // Normaliza claims con la misma factoría que Relaxed
      var jwt = validatedToken as JwtSecurityToken ?? new JwtSecurityToken(jwtRaw);
      var opts = new JwtClaimsFactory.Options
      {
        AuthType = "Bearer-Strict",
        AuthModeTag = "strict",
        ValidateLifetime = false, // ya se validó arriba
        IncludeGroups = true,
        IncludeWids = true,
        IncludeRoles = true,
        IncludeScope = true
      };

      var normalized = JwtClaimsFactory.CreateIdentity(jwt, opts);
      return normalized is null ? null : new ClaimsPrincipal(normalized);
    }
    catch (SecurityTokenValidationException)
    {
      return null;
    }
    catch
    {
      return null;
    }
  }
}
