using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface ITokenClaimExtractor
    {
        JwtSecurityToken? ReadJwt(string jwtToken);
        List<string>? GetUserGroupsOrRoles(string jwtToken);
        string? GetUserName(string jwtToken);
        string? GetUserObjectId(string jwtToken);
    }
}
