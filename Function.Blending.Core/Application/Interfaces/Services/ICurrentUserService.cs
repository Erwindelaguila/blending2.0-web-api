namespace Function.Blending.Core.Application.Interfaces.Services;

/// <summary>
/// Interface para el servicio de usuario actual
/// Simplificada para trabajar con headers de APIM/Gateway
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtiene el ID del usuario actual desde headers
    /// </summary>
    Guid GetCurrentUserId(object request);
    
    /// <summary>
    /// Obtiene el nombre del usuario actual desde headers
    /// </summary>
    string GetCurrentUserName(object request);
    
    /// <summary>
    /// Obtiene el email del usuario actual desde headers
    /// </summary>
    string GetCurrentUserEmail(object request);
    
    /// <summary>
    /// Obtiene los grupos del usuario actual desde headers
    /// </summary>
    List<string> GetUserGroups(object request);
    
    /// <summary>
    /// Verifica si el usuario está autenticado
    /// </summary>
    bool IsAuthenticated(object request);
}
