namespace Function.Blending.Core.Application.Interfaces.Services;

/// <summary>
/// Servicio para obtener información del usuario autenticado
/// Abstrae la lógica de extracción de tokens para mantener el dominio limpio
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtiene el ID del usuario autenticado desde el request
    /// </summary>
    /// <param name="request">Request que contiene el token de autorización</param>
    /// <returns>GUID del usuario autenticado</returns>
    /// <exception cref="UnauthorizedAccessException">Si el usuario no está autenticado o el token es inválido</exception>
    Guid GetCurrentUserId(object request);
    
    /// <summary>
    /// Obtiene el nombre del usuario autenticado desde el request
    /// </summary>
    /// <param name="request">Request que contiene el token de autorización</param>
    /// <returns>Nombre del usuario o null si no está disponible</returns>
    string? GetCurrentUserName(object request);
    
    /// <summary>
    /// Verifica si el request contiene un usuario autenticado válido
    /// </summary>
    /// <param name="request">Request que contiene el token de autorización</param>
    /// <returns>True si está autenticado y el token es válido</returns>
    bool IsAuthenticated(object request);
}
