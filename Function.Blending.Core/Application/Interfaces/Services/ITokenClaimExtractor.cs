using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Core.Application.Interfaces.Services
{
    /// <summary>
    /// Interfaz para extraer claims de tokens JWT
    /// </summary>
    public interface ITokenClaimExtractor
    {
        /// <summary>
        /// Lee y analiza un token JWT
        /// </summary>
        JwtSecurityToken? ReadJwt(string jwtToken);

        /// <summary>
        /// Obtiene el valor de un claim específico
        /// </summary>
        string? GetClaimValue(string jwtToken, string claimType);

        /// <summary>
        /// Obtiene los grupos o roles del usuario
        /// </summary>
        List<string>? GetUserGroupsOrRoles(string jwtToken);

        /// <summary>
        /// Obtiene el nombre del usuario
        /// </summary>
        string? GetUserName(string jwtToken);

        /// <summary>
        /// Obtiene el ID único del usuario
        /// </summary>
        string? GetUserObjectId(string jwtToken);
    }
}
