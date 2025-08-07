
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Function.Blending.Core.Infrastructure.Services
{
       public class TokenValidator : ITokenValidator
    {
        private readonly string _tenantId;
        private readonly bool _isDevelopment;
        private readonly ITokenClaimExtractor _claimExtractor;
        private readonly IAzureSigningKeyProvider _signingKeyProvider;
        private readonly ILogger<TokenValidator> _logger;

        public TokenValidator(
            ITokenClaimExtractor claimExtractor,
            IAzureSigningKeyProvider signingKeyProvider,
            ILogger<TokenValidator> logger)
        {
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
            _signingKeyProvider = signingKeyProvider ?? throw new ArgumentNullException(nameof(signingKeyProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _tenantId = Environment.GetEnvironmentVariable("AzureAD_TenantId") ?? "";
            _isDevelopment = Environment.GetEnvironmentVariable("Environment") == "Development";
        }

        public async Task<bool> ValidateTokenAsync(string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                {
                    _logger.LogWarning("Token validation failed: Token is null or empty");
                    return false;
                }

                _logger.LogDebug("Starting token validation. Environment: {Environment}", 
                    _isDevelopment ? "Development" : "Production");

                if (_isDevelopment)
                {
                    return ValidateDevelopmentToken(jwtToken);
                }
                
                return await ValidateTokenWithAzureADAsync(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unexpected error during token validation");
                return false;
            }
        }

       
        private bool ValidateDevelopmentToken(string jwtToken)
        {
            try
            {
                var jwt = _claimExtractor.ReadJwt(jwtToken);
                if (jwt == null)
                {
                    _logger.LogWarning("Development token validation failed: Unable to parse JWT token");
                    return false;
                }

                var hasOid = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Oid);
                var hasNameClaim = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Name);
                var hasUpnClaim = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Upn);
                var hasUniqueNameClaim = jwt.Claims.Any(c => c.Type == JwtClaimTypes.UniqueName);
                var hasName = hasNameClaim || hasUpnClaim || hasUniqueNameClaim;
                var notExpired = jwt.ValidTo > DateTime.UtcNow;

                if (!hasOid)
                {
                    _logger.LogWarning("Development token validation failed: Missing 'oid' claim");
                    return false;
                }

                if (!hasName)
                {
                    _logger.LogWarning("Development token validation failed: Missing name claims (name, upn, or unique_name)");
                    return false;
                }

                if (!notExpired)
                {
                    _logger.LogWarning("Development token validation failed: Token is expired. ValidTo: {ValidTo}, Current: {Current}", 
                        jwt.ValidTo, DateTime.UtcNow);
                    return false;
                }

                _logger.LogDebug("Development token validation successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during development token validation");
                return false;
            }
        }

        
        private async Task<bool> ValidateTokenWithAzureADAsync(string jwtToken)
        {
            try
            {
                _logger.LogDebug("Starting Azure AD token validation for tenant: {TenantId}", _tenantId);
                
                var handler = new JwtSecurityTokenHandler();
                var signingKeys = await _signingKeyProvider.GetSigningKeysAsync();
                
                if (!signingKeys.Any())
                {
                    _logger.LogWarning("Azure AD token validation failed: No signing keys available");
                    return false;
                }

                _logger.LogDebug("Retrieved {KeyCount} signing keys from Azure AD", signingKeys.Count());
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = new[]
                    {
                        $"https://login.microsoftonline.com/{_tenantId}/v2.0",
                        $"https://sts.windows.net/{_tenantId}/"
                    },
                    
                    ValidateAudience = false,
                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = signingKeys,
                    
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    
                    RequireSignedTokens = true,
                    RequireExpirationTime = true
                };

                handler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
                
                var jwt = validatedToken as JwtSecurityToken;
                var hasOid = jwt?.Claims.Any(c => c.Type == JwtClaimTypes.Oid) ?? false;
                var hasGroups = jwt?.Claims.Any(c => c.Type == JwtClaimTypes.Groups) ?? false;
                
                if (!hasOid)
                {
                    _logger.LogWarning("Azure AD token validation failed: Missing 'oid' claim");
                    return false;
                }

                if (!hasGroups)
                {
                    _logger.LogWarning("Azure AD token validation failed: Missing 'groups' claim");
                    return false;
                }

                _logger.LogDebug("Azure AD token validation successful");
                return true;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning(ex, "Azure AD token validation failed: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Azure AD token validation");
                return false;
            }
        }
    }
}
