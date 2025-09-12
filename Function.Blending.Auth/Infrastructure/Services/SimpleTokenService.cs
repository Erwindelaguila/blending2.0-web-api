using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class SimpleTokenService : ITokenService
    {
        private readonly ILogger<SimpleTokenService> _logger;
        private readonly ITokenClaimExtractor _claimExtractor;
        private readonly ITokenClaimValidator _claimValidator;
        private readonly ITokenSignatureValidator _signatureValidator;
        private readonly ITokenConfigurationService _config;

        public SimpleTokenService(
            ILogger<SimpleTokenService> logger,
            ITokenClaimExtractor claimExtractor,
            ITokenClaimValidator claimValidator,
            ITokenSignatureValidator signatureValidator,
            ITokenConfigurationService config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
            _claimValidator = claimValidator ?? throw new ArgumentNullException(nameof(claimValidator));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
            _config = config ?? throw new ArgumentNullException(nameof(config));
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
                if (_config.IsDevelopmentMode)
                {
                    _logger.LogDebug("Modo desarrollo: validando claims básicos únicamente");
                    return ValidateBasicClaims(jwtToken);
                }

                // En producción, validar firma criptográfica completa
                _logger.LogDebug("Modo producción: validando firma criptográfica completa");
                return await _signatureValidator.ValidateTokenSignatureAsync(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación del token");
                return false;
            }
        }

        public string? GetUserObjectId(string jwtToken)
        {
            return _claimExtractor.GetUserObjectId(jwtToken);
        }

        public string? GetUserName(string jwtToken)
        {
            return _claimExtractor.GetUserName(jwtToken);
        }

        public List<string>? GetUserGroups(string jwtToken)
        {
            return _claimExtractor.GetUserGroupsOrRoles(jwtToken);
        }

        private bool ValidateBasicClaims(string jwtToken)
        {
            try
            {
                var jwt = _claimExtractor.ReadJwt(jwtToken);
                if (jwt == null)
                {
                    _logger.LogWarning("No se pudo analizar el token JWT");
                    return false;
                }

                // Ejecutar todas las validaciones usando el validador especializado
                var validations = new[]
                {
                    _claimValidator.IsValidTokenType(jwt),
                    _claimValidator.HasValidUserIdentifier(jwt),
                    _claimValidator.IsNotExpired(jwt),
                    _claimValidator.HasValidAudience(jwt, _config.AllowedClientIds),
                    _claimValidator.IsFromAuthorizedClient(jwt, _config.AllowedClientIds),
                    _claimValidator.HasRequiredScopes(jwt)
                };

                var isValid = validations.All(v => v);
                
                if (isValid)
                {
                    _logger.LogDebug("Validación básica del token completada exitosamente");
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación básica del token");
                return false;
            }
        }
    }
}
