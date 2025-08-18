using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface ITokenClaimValidator
    {
        bool IsValidTokenType(JwtSecurityToken jwt);
        bool HasValidUserIdentifier(JwtSecurityToken jwt);
        bool IsNotExpired(JwtSecurityToken jwt);
        bool HasValidAudience(JwtSecurityToken jwt, string expectedClientId);
        bool IsFromAuthorizedClient(JwtSecurityToken jwt, List<string> allowedClientIds);
        bool HasRequiredScopes(JwtSecurityToken jwt);
    }
}
