using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class TokenSignatureValidator : ITokenSignatureValidator
    {
        private readonly ILogger<TokenSignatureValidator> _logger;
        private readonly ITokenConfigurationService _config;
        private readonly ITokenClaimValidator _claimValidator;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public TokenSignatureValidator(
            ILogger<TokenSignatureValidator> logger,
            ITokenConfigurationService config,
            ITokenClaimValidator claimValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _claimValidator = claimValidator ?? throw new ArgumentNullException(nameof(claimValidator));
            
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{_config.Authority}/.well-known/openid_configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
        }

        public async Task<bool> ValidateTokenSignatureAsync(string jwtToken)
        {
            try
            {
                var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://login.microsoftonline.com/{_config.TenantId}/v2.0",
                    
                    ValidateAudience = true,
                    ValidAudiences = new[] { $"api://{_config.ExpectedClientId}" },
                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = config.SigningKeys,
                    
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

                // Validaciones adicionales post-firma
                var jwt = validatedToken as JwtSecurityToken;
                if (jwt == null)
                {
                    _logger.LogWarning("Token validado no es un JWT válido");
                    return false;
                }

                // Usar el validador de claims para validaciones específicas
                var postSignatureValidations = new[]
                {
                    _claimValidator.IsFromAuthorizedClient(jwt, _config.AllowedClientIds),
                    _claimValidator.HasRequiredScopes(jwt)
                };

                var isValid = postSignatureValidations.All(v => v);
                
                if (isValid)
                {
                    _logger.LogDebug("Validación criptográfica del token completada exitosamente");
                }

                return isValid;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("El token ha expirado: {Message}", ex.Message);
                return false;
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                _logger.LogWarning("Audiencia del token inválida: {Message}", ex.Message);
                return false;
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                _logger.LogWarning("Emisor del token inválido: {Message}", ex.Message);
                return false;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogWarning("Firma del token inválida: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación criptográfica del token");
                return false;
            }
        }
    }
}
