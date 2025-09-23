using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Services
{
   
    public class TokenClaimExtractor : ITokenClaimExtractor
    {
        private readonly ILogger<TokenClaimExtractor> _logger;

        public TokenClaimExtractor(ILogger<TokenClaimExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public JwtSecurityToken? ReadJwt(string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al analizar el token JWT");
                return null;
            }
        }

        public List<string>? GetUserGroupsOrRoles(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            // Buscar grupos en el claim 'groups'
            var groupClaims = jwt.Claims
                .Where(c => c.Type == JwtClaimTypes.Groups)
                .Select(c => c.Value)
                .ToList();

            if (groupClaims.Any())
                return groupClaims;

            // Como fallback, buscar en WIDS (roles de directorio)
            var widsClaims = jwt.Claims
                .Where(c => c.Type == JwtClaimTypes.Wids)
                .Select(c => c.Value)
                .ToList();

            if (widsClaims.Any())
                return widsClaims;

            return null;
        }

        public string? GetUserObjectId(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            // Preferir oid, pero usar sub como fallback
            var objectId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Oid)?.Value;
            if (string.IsNullOrEmpty(objectId))
            {
                objectId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Sub)?.Value;
            }
            
            return objectId;
        }

        public string? GetUserName(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            // Buscar nombre en los claims disponibles por prioridad para access tokens
            var userName = jwt.Claims.FirstOrDefault(c =>
                c.Type == JwtClaimTypes.Name ||
                c.Type == JwtClaimTypes.PreferredUsername ||
                c.Type == JwtClaimTypes.Upn ||
                c.Type == JwtClaimTypes.UniqueName)?.Value;

            return userName;
        }

        public List<string>? GetUserScopes(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            // Buscar scopes en el claim 'scp'
            var scopeClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Scope)?.Value;
            if (string.IsNullOrEmpty(scopeClaim))
                return new List<string>();

            // Los scopes vienen separados por espacios
            return scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
