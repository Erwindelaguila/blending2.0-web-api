using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Auth.Application.Interfaces.Services;

/// <summary>
/// Interface para el servicio de extracción de información del usuario desde headers
/// </summary>
public interface IHeaderUserService
{
    /// <summary>
    /// Obtiene el ID del usuario desde headers APIM
    /// </summary>
    Guid GetCurrentUserId(HttpRequestData request);
    
    /// <summary>
    /// Obtiene el nombre del usuario desde headers APIM
    /// </summary>
    string GetCurrentUserName(HttpRequestData request);
    
    /// <summary>
    /// Obtiene el email del usuario desde headers APIM
    /// </summary>
    string GetCurrentUserEmail(HttpRequestData request);
    
    /// <summary>
    /// Obtiene los grupos del usuario desde headers APIM
    /// </summary>
    List<string> GetUserGroups(HttpRequestData request);
    
    /// <summary>
    /// Verifica si el usuario tiene un grupo específico
    /// </summary>
    bool HasGroup(HttpRequestData request, string groupName);
    
    /// <summary>
    /// Verifica si el usuario tiene alguno de los grupos especificados
    /// </summary>
    bool HasAnyGroup(HttpRequestData request, params string[] groupNames);
}
