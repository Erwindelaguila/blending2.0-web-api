using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class BasicTokenService : ITokenService
    {
        private readonly ILogger<BasicTokenService> _logger;
        private readonly ITokenClaimExtractor _claimExtractor;

        public BasicTokenService(
            ILogger<BasicTokenService> logger,
            ITokenClaimExtractor claimExtractor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
        }

        public Task<bool> ValidateTokenAsync(string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                {
                    _logger.LogWarning("Validación del token falló: el token está vacío o es nulo");
                    return Task.FromResult(false);
                }

                _logger.LogDebug("Iniciando validación básica del token JWT (solo deserialización)");

                // Solo validación básica: el token se puede deserializar
                var jwt = _claimExtractor.ReadJwt(jwtToken);
                if (jwt == null)
                {
                    _logger.LogWarning("El token no es válido o no se puede deserializar");
                    return Task.FromResult(false);
                }

                _logger.LogDebug("Token JWT deserializado exitosamente");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación del token");
                return Task.FromResult(false);
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
    }
}