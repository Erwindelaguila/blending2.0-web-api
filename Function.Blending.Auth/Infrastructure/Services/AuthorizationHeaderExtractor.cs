using System;
using System.Linq;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class AuthorizationHeaderExtractor : IAuthorizationHeaderExtractor
    {
        private readonly ILogger<AuthorizationHeaderExtractor> _logger;
        private const string AuthorizationHeader = "Authorization";
        private const string BearerPrefix = "Bearer ";

        public AuthorizationHeaderExtractor(ILogger<AuthorizationHeaderExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string? ExtractJwtToken(HttpRequestData request)
        {
            if (!request.Headers.TryGetValues(AuthorizationHeader, out var authHeaders))
            {
                _logger.LogWarning("Header Authorization no encontrado");
                return null;
            }

            var authHeader = authHeaders.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(BearerPrefix))
            {
                _logger.LogWarning("Formato de Authorization header inválido");
                return null;
            }

            var jwtToken = authHeader.Substring(BearerPrefix.Length).Trim();
            _logger.LogDebug("Token JWT extraído exitosamente");
            
            return jwtToken;
        }
    }
}
