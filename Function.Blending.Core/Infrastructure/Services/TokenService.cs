using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Blending.Core.Application.Interfaces.Services;

namespace Function.Blending.Core.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly ITokenClaimExtractor _claimExtractor;

        public TokenService(
            ITokenValidator tokenValidator,
            ITokenClaimExtractor claimExtractor)
        {
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _claimExtractor = claimExtractor ?? throw new ArgumentNullException(nameof(claimExtractor));
        }

        public async Task<bool> ValidateTokenAsync(string jwtToken)
        {
            return await _tokenValidator.ValidateTokenAsync(jwtToken);
        }

        public string? GetClaimValue(string jwtToken, string claimType)
        {
            return _claimExtractor.GetClaimValue(jwtToken, claimType);
        }

        public List<string>? GetUserGroups(string jwtToken)
        {
            return _claimExtractor.GetUserGroupsOrRoles(jwtToken);
        }

        public string? GetUserName(string jwtToken)
        {
            return _claimExtractor.GetUserName(jwtToken);
        }

        public string? GetUserLastName(string jwtToken)
        {
            return _claimExtractor.GetUserLastName(jwtToken);
        }

        public string? GetUserObjectId(string jwtToken)
        {
            return _claimExtractor.GetUserObjectId(jwtToken);
        }

        public string? GetUserEmail(string jwtToken)
        {
            return _claimExtractor.GetUserEmail(jwtToken);
        }

        public bool IsGraphToken(string jwtToken)
        {
            return _claimExtractor.IsGraphToken(jwtToken);
        }
    }
}
