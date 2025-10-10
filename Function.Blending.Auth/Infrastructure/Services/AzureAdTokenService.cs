using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Common;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class AzureAdTokenService : ITokenService
    {
        private readonly ILogger<AzureAdTokenService> _logger;
        private readonly ITokenClaimExtractor _claimExtractor;
        private readonly AzureAdConfiguration _azureAdConfig;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly bool _allowOfflineValidation;
        private readonly IConfiguration _configuration;

        public AzureAdTokenService(ILogger<AzureAdTokenService> logger, ITokenClaimExtractor claimExtractor, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            // Configuración desde settings (usando formato Azure Portal)
            var tenantId = configuration["AzureAd__TenantId"] ?? configuration["AzureAd:TenantId"] ?? throw new InvalidOperationException("AzureAd TenantId no configurado");
            var clientId = configuration["AzureAd__ClientId"] ?? configuration["AzureAd:ClientId"] ?? throw new InvalidOperationException("AzureAd ClientId no configurado");
            
            _azureAdConfig = new AzureAdConfiguration
            {
                TenantId = tenantId,
                ClientId = clientId
            };
            
            // Permitir o no validación offline (fallback). Por defecto, deshabilitado en Producción.
            var env = configuration["Environment"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var allowOfflineSetting = configuration["AzureAd__AllowOfflineValidation"] ?? configuration["AzureAd:AllowOfflineValidation"];
            if (!string.IsNullOrEmpty(allowOfflineSetting) && bool.TryParse(allowOfflineSetting, out var allowOfflineFlag))
            {
                _allowOfflineValidation = allowOfflineFlag;
            }
            else
            {
                _allowOfflineValidation = !string.Equals(env, "Production", StringComparison.OrdinalIgnoreCase);
            }
            
            // Configurar el manager para obtener las claves públicas de Azure AD usando Authority de configuración
            var authority = configuration["AzureAd__Authority"] ?? configuration["AzureAd:Authority"] ?? $"https://login.microsoftonline.com/{tenantId}";
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
        }

        public async Task<bool> ValidateTokenAsync(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                {
                    _logger.LogWarning("Token JWT está vacío o es nulo");
                    return false;
                }
                
                var handler = new JwtSecurityTokenHandler
                {
                    // Mantener los nombres de claims originales del JWT ("oid", "tid", etc.)
                    MapInboundClaims = false
                };
                
                if (!handler.CanReadToken(jwtToken))
                {
                    _logger.LogWarning("Token JWT no es válido o no se puede leer");
                    return false;
                }
                
                // Leer el token para obtener información básica
                var jsonToken = handler.ReadJwtToken(jwtToken);
                
                // Configurar parámetros de validación
                var validAudiencesConfig = _configuration["AzureAd__ValidAudiences"] ?? _configuration["AzureAd:ValidAudiences"];
                var validAudiences = !string.IsNullOrEmpty(validAudiencesConfig) 
                    ? validAudiencesConfig.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string> { _azureAdConfig.ClientId, $"api://{_azureAdConfig.ClientId}" };
                var validIssuers = new List<string> 
                { 
                    $"https://login.microsoftonline.com/{_azureAdConfig.TenantId}/v2.0",
                    $"https://sts.windows.net/{_azureAdConfig.TenantId}/"
                };
                
                try
                {
                    // INTENTAR VALIDACIÓN COMPLETA CON FIRMA
                    _logger.LogInformation("Intentando validación completa del token JWT con firma");
                    
                    // Obtener configuración OpenID Connect con timeout
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var openIdConfig = await _configurationManager.GetConfigurationAsync(cts.Token);
                    
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuers = validIssuers,
                        ValidateAudience = true,
                        ValidAudiences = validAudiences,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = openIdConfig.SigningKeys,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5),
                        RequireExpirationTime = true,
                        RequireSignedTokens = true
                    };
                    
                    // Validar token completo con firma
                    var principal = handler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);
                    
                    // Obtener datos de usuario de forma robusta (principal y token crudo)
                    var userId = principal.FindFirst("oid")?.Value 
                                 ?? principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
                    var userName = principal.FindFirst("name")?.Value ?? principal.FindFirst("preferred_username")?.Value;

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                    {
                        var raw = handler.ReadJwtToken(jwtToken);
                        userId = userId 
                                 ?? raw.Claims.FirstOrDefault(c => c.Type == "oid")?.Value
                                 ?? raw.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                                 ?? raw.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                        userName = userName 
                                   ?? raw.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                                   ?? raw.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
                                   ?? raw.Claims.FirstOrDefault(c => c.Type == "upn")?.Value;
                    }
                    
                    _logger.LogInformation($"✅ TOKEN VALIDADO COMPLETAMENTE (con firma) para usuario: {userName} (ID: {userId})");
                    return true;
                }
                catch (Exception ex)
                {
                    // Si falla la validación completa, hacer validación básica
                    if (!_allowOfflineValidation)
                    {
                        _logger.LogError($"❌ Validación con firma falló y la validación offline está DESHABILITADA. Motivo: {ex.GetType().Name}: {ex.Message}");
                        return false;
                    }
                    _logger.LogWarning($"⚠️  Validación completa falló ({ex.GetType().Name}: {ex.Message}). Aplicando validación básica...");
                    
                    // VALIDACIÓN BÁSICA (FALLBACK)
                    
                    // 1. Verificar expiración
                    var currentTime = DateTimeOffset.UtcNow;
                    if (jsonToken.ValidTo <= currentTime.DateTime)
                    {
                        _logger.LogError($"❌ Token JWT ha expirado. Expiró: {jsonToken.ValidTo}, Actual: {currentTime}");
                        return false;
                    }
                    
                    // 2. Verificar audience
                    var audience = jsonToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                    if (string.IsNullOrEmpty(audience) || !validAudiences.Contains(audience))
                    {
                        _logger.LogError($"❌ Audience inválida: {audience}. Esperadas: {string.Join(", ", validAudiences)}");
                        return false;
                    }
                    
                    // 3. Verificar issuer
                    var issuer = jsonToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                    if (string.IsNullOrEmpty(issuer) || !validIssuers.Contains(issuer))
                    {
                        _logger.LogError($"❌ Issuer inválido: {issuer}. Esperados: {string.Join(", ", validIssuers)}");
                        return false;
                    }
                    
                    // 4. Verificar tenant ID
                    var tokenTenantId = jsonToken.Claims.FirstOrDefault(c => c.Type == "tid")?.Value;
                    if (tokenTenantId != _azureAdConfig.TenantId)
                    {
                        _logger.LogError($"❌ Tenant ID incorrecto: {tokenTenantId}. Esperado: {_azureAdConfig.TenantId}");
                        return false;
                    }
                    
                    // 5. Verificar claims básicos
                    var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
                    var userName = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? 
                                   jsonToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                    
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogError("❌ Token no contiene Object ID (oid) del usuario");
                        return false;
                    }
                    
                    _logger.LogWarning($"⚠️  Token validado básicamente (SIN FIRMA) para usuario: {userName} (ID: {userId})");
                    _logger.LogWarning("⚠️  ADVERTENCIA: La firma del token NO se verificó - Solo para desarrollo");
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error crítico durante validación del token: {ex.Message}");
                return false;
            }
        }

        public UserClaims ExtractUserClaims(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return new UserClaims();
                }

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwtToken))
                {
                    return new UserClaims();
                }

                var jsonToken = handler.ReadJwtToken(jwtToken);
                
                return new UserClaims
                {
                    ObjectId = jsonToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value,
                    Name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                           jsonToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                    Groups = jsonToken.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error extrayendo claims del token: {ex.Message}");
                return new UserClaims();
            }
        }

        public bool IsTokenExpired(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return true;
                }

                var handler = new JwtSecurityTokenHandler();
                
                if (!handler.CanReadToken(jwtToken))
                {
                    return true;
                }

                var jsonToken = handler.ReadJwtToken(jwtToken);
                var currentTime = DateTimeOffset.UtcNow;
                
                return jsonToken.ValidTo <= currentTime.DateTime;
            }
            catch
            {
                return true;
            }
        }

        public string? GetUserObjectId(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwtToken))
                    return null;

                var jsonToken = handler.ReadJwtToken(jwtToken);
                return jsonToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo ObjectId del token: {ex.Message}");
                return null;
            }
        }

        public string? GetUserName(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwtToken))
                    return null;

                var jsonToken = handler.ReadJwtToken(jwtToken);
                
                // Probar diferentes claims de nombre
                return jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                       jsonToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ??
                       jsonToken.Claims.FirstOrDefault(c => c.Type == "upn")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo nombre de usuario del token: {ex.Message}");
                return null;
            }
        }

        public List<string>? GetUserGroups(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwtToken))
                    return null;

                var jsonToken = handler.ReadJwtToken(jwtToken);
                
                // Obtener grupos del token
                var groupClaims = jsonToken.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
                
                return groupClaims.Any() ? groupClaims : null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo grupos del usuario del token: {ex.Message}");
                return null;
            }
        }
    }
}