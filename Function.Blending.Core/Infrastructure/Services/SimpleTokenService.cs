using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Constants;

namespace Function.Blending.Core.Infrastructure.Services
{
    /// <summary>
    /// Servicio simple de validación de tokens que lee claims directos del JWT
    /// Incluye validación criptográfica contra las claves públicas de Microsoft
    /// </summary>
    public class SimpleTokenService : ITokenService
    {
        private readonly ILogger<SimpleTokenService> _logger;
        private readonly ITokenClaimExtractor _claimExtractor;
        private readonly bool _isDevelopment;
        private readonly string _tenantId;
        private readonly string _expectedClientId;
        private readonly List<string> _allowedClientIds;
        private readonly string _authority;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public SimpleTokenService(
            ILogger<SimpleTokenService> logger,
            ITokenClaimExtractor claimExtractor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
            
            _isDevelopment = Environment.GetEnvironmentVariable("Environment") == "Development";
            
            _tenantId = Environment.GetEnvironmentVariable("AzureAD_TenantId") 
                ?? throw new InvalidOperationException("La variable de entorno 'AzureAD_TenantId' es requerida");
            
            _expectedClientId = Environment.GetEnvironmentVariable("AzureAD_ClientId") 
                ?? throw new InvalidOperationException("La variable de entorno 'AzureAD_ClientId' es requerida");
            
            var allowedClientIdsString = Environment.GetEnvironmentVariable("AzureAD_AllowedClientIds") ?? _expectedClientId;
            _allowedClientIds = allowedClientIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim()).ToList();
            
            // URL de las claves públicas de Microsoft
            _authority = $"https://login.microsoftonline.com/{_tenantId}/v2.0";
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{_authority}/.well-known/openid_configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
        }

        public async Task<bool> ValidateTokenAsync(string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                {
                    _logger.LogWarning("Validación del token falló: el token está vacío o es nulo");
                    return false;
                }

                _logger.LogDebug("Iniciando validación del token JWT");

                // En desarrollo, solo validar claims básicos sin firma
                if (_isDevelopment)
                {
                    _logger.LogDebug("Modo desarrollo: validando claims básicos únicamente");
                    return await ValidateBasicClaimsAsync(jwtToken);
                }

                // En producción, validar firma criptográfica completa
                _logger.LogDebug("Modo producción: validando firma criptográfica completa");
                return await ValidateTokenWithSignatureAsync(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación del token");
                return false;
            }
        }

        private async Task<bool> ValidateBasicClaimsAsync(string jwtToken)
        {
            try
            {
                var jwt = _claimExtractor.ReadJwt(jwtToken);
                if (jwt == null)
                {
                    _logger.LogWarning("No se pudo analizar el token JWT");
                    return false;
                }

                // 1. Rechazar ID tokens - solo aceptar access tokens
                var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.TokenType)?.Value;
                if (tokenType?.ToLower() == "id_token")
                {
                    _logger.LogWarning("Se recibió un ID token pero solo se aceptan access tokens");
                    return false;
                }

                // 2. Validar que tenga oid o sub (identificador del usuario)
                var hasOid = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Oid);
                var hasSub = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Sub);
                if (!hasOid && !hasSub)
                {
                    _logger.LogWarning("El token no contiene claim 'oid' ni 'sub' requerido");
                    return false;
                }

                // 3. Verificar que no ha expirado
                var notExpired = jwt.ValidTo > DateTime.UtcNow;
                if (!notExpired)
                {
                    _logger.LogWarning("El token ha expirado. Válido hasta: {ValidTo}, Actual: {Current}", jwt.ValidTo, DateTime.UtcNow);
                    return false;
                }

                // 4. Validar audience - debe ser api://{BACKEND_CLIENT_ID}
                var audienceClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Audience)?.Value;
                var expectedApiAudience = $"api://{_expectedClientId}";
                if (audienceClaim != expectedApiAudience)
                {
                    _logger.LogWarning("Audiencia del token inválida. Esperada: {ExpectedAudience}, Recibida: {Audience}", expectedApiAudience, audienceClaim);
                    return false;
                }

                // 5. Verificar que viene de SPA autorizada (azp/appid en lista blanca)
                var azp = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AuthorizedParty)?.Value;
                var appId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AppId)?.Value;
                var clientId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.ClientId)?.Value;
                
                var originClientId = azp ?? appId ?? clientId;
                if (string.IsNullOrEmpty(originClientId) || !_allowedClientIds.Contains(originClientId))
                {
                    _logger.LogWarning("Cliente no autorizado. Cliente: {ClientId}, Lista permitida: {AllowedClients}", 
                        originClientId, string.Join(", ", _allowedClientIds));
                    return false;
                }

                // 6. Verificar scope access_as_user
                var scopes = jwt.Claims.Where(c => c.Type == JwtClaimTypes.Scope)
                    .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .ToList();
                
                if (!scopes.Contains("access_as_user"))
                {
                    _logger.LogWarning("El token no contiene el scope requerido 'access_as_user'. Scopes: {Scopes}", 
                        string.Join(", ", scopes));
                    return false;
                }

                _logger.LogDebug("Validación básica del token completada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación básica del token");
                return false;
            }
        }

        private async Task<bool> ValidateTokenWithSignatureAsync(string jwtToken)
        {
            try
            {
                var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://login.microsoftonline.com/{_tenantId}/v2.0",
                    
                    ValidateAudience = true,
                    ValidAudiences = new[] { $"api://{_expectedClientId}" }, // Solo audience de API
                    
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

                // Verificar que viene de cliente autorizado
                var azp = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AuthorizedParty)?.Value;
                var appId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AppId)?.Value;
                var clientId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.ClientId)?.Value;
                
                var originClientId = azp ?? appId ?? clientId;
                if (string.IsNullOrEmpty(originClientId) || !_allowedClientIds.Contains(originClientId))
                {
                    _logger.LogWarning("Cliente no autorizado después de validación de firma. Cliente: {ClientId}", originClientId);
                    return false;
                }

                // Verificar scope access_as_user
                var scopes = jwt.Claims.Where(c => c.Type == JwtClaimTypes.Scope)
                    .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .ToList();
                
                if (!scopes.Contains("access_as_user"))
                {
                    _logger.LogWarning("Token sin scope requerido después de validación de firma. Scopes: {Scopes}", 
                        string.Join(", ", scopes));
                    return false;
                }

                _logger.LogDebug("Validación criptográfica del token completada exitosamente");
                return true;
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

        public string GetUserObjectId(string jwtToken)
        {
            return _claimExtractor.GetUserObjectId(jwtToken) ?? string.Empty;
        }

        public string GetUserName(string jwtToken)
        {
            return _claimExtractor.GetUserName(jwtToken) ?? string.Empty;
        }

        public List<string> GetUserGroups(string jwtToken)
        {
            return _claimExtractor.GetUserGroupsOrRoles(jwtToken) ?? new List<string>();
        }

        public string GetClaimValue(string jwtToken, string claimType)
        {
            return _claimExtractor.GetClaimValue(jwtToken, claimType) ?? string.Empty;
        }
    }
}
