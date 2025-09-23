using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class TokenClaimValidator : ITokenClaimValidator
    {
        private readonly ILogger<TokenClaimValidator> _logger;
        private readonly ITokenConfigurationService _config;

        public TokenClaimValidator(ILogger<TokenClaimValidator> logger, ITokenConfigurationService config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool IsValidTokenType(JwtSecurityToken jwt)
        {
            var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.TokenType)?.Value;
            if (tokenType?.ToLower() == "id_token")
            {
                _logger.LogWarning("Se recibió un ID token pero solo se aceptan access tokens");
                return false;
            }
            return true;
        }

        public bool HasValidUserIdentifier(JwtSecurityToken jwt)
        {
            var hasOid = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Oid);
            var hasSub = jwt.Claims.Any(c => c.Type == JwtClaimTypes.Sub);
            
            if (!hasOid && !hasSub)
            {
                _logger.LogWarning("El token no contiene claim 'oid' ni 'sub' requerido");
                return false;
            }
            return true;
        }

        public bool IsNotExpired(JwtSecurityToken jwt)
        {
            var notExpired = jwt.ValidTo > DateTime.UtcNow;
            if (!notExpired)
            {
                _logger.LogWarning("El token ha expirado. Válido hasta: {ValidTo}, Actual: {Current}", 
                    jwt.ValidTo, DateTime.UtcNow);
                return false;
            }
            return true;
        }

        public bool HasValidAudience(JwtSecurityToken jwt, List<string> allowedClientIds)
        {
            var audienceClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Audience)?.Value;
            
            if (string.IsNullOrEmpty(audienceClaim))
            {
                _logger.LogWarning("El token no contiene claim de audiencia");
                return false;
            }

            // Verificar si la audiencia coincide con alguno de los Client IDs permitidos
            foreach (var clientId in allowedClientIds)
            {
                var expectedApiAudience = $"api://{clientId}";
                
                // Validación configurable: permitir formato específico según configuración
                var audienceFormat = Environment.GetEnvironmentVariable("AzureAD_ExpectedAudienceFormat") ?? "Both";
                
                bool isValidAudience = audienceFormat.ToLower() switch
                {
                    "apionly" => audienceClaim == expectedApiAudience,
                    "clientidonly" => audienceClaim == clientId,
                    "both" => audienceClaim == expectedApiAudience || audienceClaim == clientId,
                    _ => audienceClaim == expectedApiAudience || audienceClaim == clientId
                };
                
                if (isValidAudience)
                {
                    return true;
                }
            }

            var allowedFormats = allowedClientIds.SelectMany(id => new[] { $"api://{id}", id }).ToList();
            _logger.LogWarning("Audiencia del token inválida. Recibida: {Audience}, Permitidas: {AllowedAudiences}", 
                audienceClaim, string.Join(", ", allowedFormats));
            return false;
        }

        public bool IsFromAuthorizedClient(JwtSecurityToken jwt, List<string> allowedClientIds)
        {
            var azp = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AuthorizedParty)?.Value;
            var appId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.AppId)?.Value;
            var clientId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.ClientId)?.Value;
            
            var originClientId = azp ?? appId ?? clientId;
            if (string.IsNullOrEmpty(originClientId) || !allowedClientIds.Contains(originClientId))
            {
                _logger.LogWarning("Cliente no autorizado. Cliente: {ClientId}, Lista permitida: {AllowedClients}", 
                    originClientId, string.Join(", ", allowedClientIds));
                return false;
            }
            return true;
        }

        public bool HasRequiredScopes(JwtSecurityToken jwt)
        {
            var scopes = jwt.Claims.Where(c => c.Type == JwtClaimTypes.Scope)
                .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();
            
            // Usar scopes configurables desde la configuración
            var hasValidScope = _config.RequiredScopes.Any(requiredScope => 
                scopes.Contains(requiredScope) || 
                scopes.Any(s => s.EndsWith(requiredScope)));
            
            if (!hasValidScope)
            {
                _logger.LogWarning("El token no contiene ninguno de los scopes requeridos. Esperados: {RequiredScopes}, Recibidos: {Scopes}", 
                    string.Join(", ", _config.RequiredScopes), string.Join(", ", scopes));
                return false;
            }
            return true;
        }
    }
}
