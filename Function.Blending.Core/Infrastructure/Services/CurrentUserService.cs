using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Infrastructure.Services;

/// <summary>
/// 🆔 CURRENT USER SERVICE - CÓDIGO IDÉNTICO EN DESARROLLO Y PRODUCCIÓN
/// Lee headers agregados por APIM (real) o ApimSimulatorMiddleware (desarrollo)
/// ✅ MISMO CÓDIGO, MISMOS HEADERS, CERO CAMBIOS AL PASAR A PRODUCCIÓN
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    /// <summary>
    /// Obtiene el ID del usuario actual desde headers APIM
    /// Header: X-User-Id (agregado por APIM o ApimSimulator)
    /// </summary>
    public Guid GetCurrentUserId(object request)
    {
        var httpRequest = request as HttpRequestData;
        var userIdStr = GetHeaderValue(httpRequest, "X-User-Id");
        
        if (Guid.TryParse(userIdStr, out var userId))
            return userId;
            
        return Guid.Empty; // Usuario anónimo
    }

    /// <summary>
    /// Obtiene el nombre del usuario actual desde headers APIM  
    /// Header: X-User-Name (agregado por APIM o ApimSimulator)
    /// </summary>
    public string GetCurrentUserName(object request)
    {
        var httpRequest = request as HttpRequestData;
        return GetHeaderValue(httpRequest, "X-User-Name") ?? "Unknown User";
    }

    /// <summary>
    /// Obtiene el email del usuario actual desde headers APIM
    /// Header: X-User-Email (agregado por APIM o ApimSimulator)
    /// </summary>
    public string GetCurrentUserEmail(object request)
    {
        var httpRequest = request as HttpRequestData;
        return GetHeaderValue(httpRequest, "X-User-Email") ?? "unknown@email.com";
    }

    /// <summary>
    /// Obtiene los grupos del usuario actual desde headers APIM
    /// Header: X-User-Groups (separados por coma)
    /// </summary>
    public List<string> GetUserGroups(object request)
    {
        var httpRequest = request as HttpRequestData;
        var groupsHeader = GetHeaderValue(httpRequest, "X-User-Groups");
        
        if (string.IsNullOrEmpty(groupsHeader))
            return new List<string>();
            
        return groupsHeader.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(g => g.Trim())
                          .ToList();
    }

    /// <summary>
    /// Verifica si el usuario está autenticado
    /// Un usuario está autenticado si tiene un X-User-Id válido
    /// </summary>
    public bool IsAuthenticated(object request)
    {
        var userId = GetCurrentUserId(request);
        return userId != Guid.Empty;
    }

    // ✅ MÉTODOS ADICIONALES PARA FUNCIONALIDAD COMPLETA

    /// <summary>
    /// Obtiene los scopes del usuario actual desde headers APIM
    /// Header: X-User-Scopes (separados por espacio)
    /// </summary>
    public string[] GetUserScopes(HttpRequestData request)
    {
        var scopesHeader = GetHeaderValue(request, "X-User-Scopes");
        
        if (string.IsNullOrEmpty(scopesHeader))
            return Array.Empty<string>();
            
        return scopesHeader.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .ToArray();
    }

    /// <summary>
    /// Obtiene el tenant ID del usuario actual desde headers APIM
    /// Header: X-User-Tenant (agregado por APIM o ApimSimulator)
    /// </summary>
    public string GetUserTenant(HttpRequestData request)
    {
        return GetHeaderValue(request, "X-User-Tenant") ?? "";
    }

    /// <summary>
    /// Verifica si el usuario tiene un scope específico
    /// </summary>
    public bool HasScope(HttpRequestData request, string scope)
    {
        var userScopes = GetUserScopes(request);
        return userScopes.Contains(scope, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica si el usuario pertenece a un grupo específico
    /// </summary>
    public bool IsInGroup(HttpRequestData request, string group)
    {
        var userGroups = GetUserGroups(request);
        return userGroups.Contains(group, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Helper para extraer header values de forma segura
    /// </summary>
    private string? GetHeaderValue(HttpRequestData? request, string headerName)
    {
        try
        {
            if (request?.Headers?.TryGetValues(headerName, out var headerValues) == true)
            {
                return headerValues.FirstOrDefault();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}