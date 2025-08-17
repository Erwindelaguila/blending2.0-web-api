
namespace Function.Blending.Core.Application.Interfaces.Services
{
    /// <summary>
    /// Servicio para validación y extracción de claims de tokens JWT
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Valida la autenticidad y validez del token JWT
        /// </summary>
        Task<bool> ValidateTokenAsync(string jwtToken);
        
        /// <summary>
        /// Obtiene el valor de un claim específico del token
        /// </summary>
        string? GetClaimValue(string jwtToken, string claimType);
        
        /// <summary>
        /// Obtiene los grupos del usuario desde el token
        /// </summary>
        List<string>? GetUserGroups(string jwtToken);
        
        /// <summary>
        /// Obtiene el nombre del usuario desde el token
        /// </summary>
        string? GetUserName(string jwtToken);
        
        /// <summary>
        /// Obtiene el ID único del usuario desde el token
        /// </summary>
        string? GetUserObjectId(string jwtToken);
    }
}
