using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Core.Application.Interfaces.Services
{

    public interface ITokenClaimExtractor
    {
 
        JwtSecurityToken? ReadJwt(string jwtToken);

        string? GetClaimValue(string jwtToken, string claimType);

        List<string>? GetUserGroupsOrRoles(string jwtToken);

        string? GetUserName(string jwtToken);

        string? GetUserLastName(string jwtToken);

        string? GetUserObjectId(string jwtToken);
        string? GetUserEmail(string jwtToken);
        bool IsGraphToken(string jwtToken);
    }
}
