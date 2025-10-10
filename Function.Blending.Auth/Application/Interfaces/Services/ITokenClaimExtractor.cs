using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface ITokenClaimExtractor
    {
        JwtSecurityToken? ReadJwt(string jwtToken);
        string? GetUserObjectId(string jwtToken);
        string? GetUserName(string jwtToken);
        List<string>? GetUserGroupsOrRoles(string jwtToken);
        List<string>? GetUserScopes(string jwtToken);
    }
}
