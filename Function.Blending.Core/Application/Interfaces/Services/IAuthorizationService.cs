namespace Function.Blending.Core.Application.Interfaces.Services;

/// <summary>
/// Servicio de autorización que valida permisos usando headers X-User-Scopes
/// exactamente como lo haría APIM en producción.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Valida si el usuario actual tiene el scope requerido para el endpoint.
    /// </summary>
    /// <param name="requiredScope">Scope requerido (ej: "appparams.read", "appparams.write")</param>
    /// <returns>True si tiene permisos, False si no</returns>
    bool HasRequiredScope(string requiredScope);
    
    /// <summary>
    /// Obtiene el User ID del usuario actual desde los headers.
    /// </summary>
    /// <returns>User ID para auditoría</returns>
    string GetCurrentUserId();
    
    /// <summary>
    /// Obtiene el nombre del usuario actual desde los headers.
    /// </summary>
    /// <returns>Nombre del usuario para logs</returns>
    string GetCurrentUserName();
    
    /// <summary>
    /// Obtiene todos los scopes del usuario actual.
    /// </summary>
    /// <returns>Lista de scopes</returns>
    string[] GetCurrentUserScopes();
}
