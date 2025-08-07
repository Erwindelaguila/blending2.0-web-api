using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Infrastructure.Services
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
                {
                    _logger.LogDebug("Cannot read JWT: Token is null or empty");
                    return null;
                }

                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to parse JWT token");
                return null;
            }
        }

        public string? GetClaimValue(string jwtToken, string claimType)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            var claim = jwt.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

        public List<string>? GetUserGroupsOrRoles(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            var groupClaims = jwt.Claims
                .Where(c => c.Type == JwtClaimTypes.Groups)
                .Select(c => c.Value)
                .ToList();

            if (groupClaims.Any())
            {
                _logger.LogDebug("Found {GroupCount} group claims in token", groupClaims.Count);
                return groupClaims;
            }


            var widsClaims = jwt.Claims
                .Where(c => c.Type == JwtClaimTypes.Wids)
                .Select(c => c.Value)
                .ToList();

            if (widsClaims.Any())
            {
                _logger.LogDebug("Found {WidsCount} WIDS claims in token as fallback", widsClaims.Count);
                return widsClaims;
            }

            _logger.LogDebug("No group or WIDS claims found in token");
            return null;
        }

        public string? GetUserName(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

           
            var userName = jwt.Claims.FirstOrDefault(c =>
                c.Type == JwtClaimTypes.Name ||
                c.Type == JwtClaimTypes.Upn ||
                c.Type == JwtClaimTypes.UniqueName)?.Value;

            _logger.LogDebug("Retrieved user name from token: {HasUserName}", !string.IsNullOrEmpty(userName));
            return userName;
        }

        public string? GetUserLastName(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            var lastName = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
            _logger.LogDebug("Retrieved user last name from token: {HasLastName}", !string.IsNullOrEmpty(lastName));
            return lastName;
        }

        public string? GetUserObjectId(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;

            var objectId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Oid)?.Value;
            _logger.LogDebug("Retrieved user object ID from token: {HasObjectId}", !string.IsNullOrEmpty(objectId));
            return objectId;
        }

        public string? GetUserEmail(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return null;


            var email = jwt.Claims.FirstOrDefault(c =>
                c.Type == JwtClaimTypes.Upn ||
                c.Type == JwtClaimTypes.Email ||
                c.Type == JwtClaimTypes.UniqueName)?.Value;

            _logger.LogDebug("Retrieved user email from token: {HasEmail}", !string.IsNullOrEmpty(email));
            return email;
        }

        public bool IsGraphToken(string jwtToken)
        {
            var jwt = ReadJwt(jwtToken);
            if (jwt == null) return false;

            var audience = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Audience)?.Value;
            var isGraphToken = audience == GraphApiConstants.MICROSOFT_GRAPH_AUDIENCE;
            
            _logger.LogDebug("Token audience check - Is Graph token: {IsGraphToken}, Audience: {Audience}", 
                isGraphToken, audience);
            
            return isGraphToken;
        }
    }
}
