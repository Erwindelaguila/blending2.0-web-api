using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services;

/// <summary>
/// Servicio para extraer información del usuario desde headers agregados por APIM
/// Reemplaza la validación JWT manual
/// </summary>
public class HeaderUserService : IHeaderUserService
{
    private readonly ILogger<HeaderUserService> _logger;

    public HeaderUserService(ILogger<HeaderUserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene el ID del usuario desde el header X-User-Id
    /// </summary>
    public Guid GetCurrentUserId(HttpRequestData request)
    {
        try
        {
            var userIdHeader = GetHeaderValue(request, "X-User-Id");
            
            if (Guid.TryParse(userIdHeader, out var userId))
            {
                _logger.LogDebug("Usuario autenticado: {UserId}", userId);
                return userId;
            }
            
            _logger.LogWarning("Header X-User-Id no encontrado o inválido");
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer ID del usuario");
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Obtiene el nombre del usuario desde el header X-User-Name
    /// </summary>
    public string GetCurrentUserName(HttpRequestData request)
    {
        return GetHeaderValue(request, "X-User-Name") ?? "Unknown User";
    }

    /// <summary>
    /// Obtiene el email del usuario desde el header X-User-Email
    /// </summary>
    public string GetCurrentUserEmail(HttpRequestData request)
    {
        return GetHeaderValue(request, "X-User-Email") ?? "unknown@email.com";
    }

    /// <summary>
    /// Obtiene los grupos del usuario desde el header X-User-Groups
    /// </summary>
    public List<string> GetUserGroups(HttpRequestData request)
    {
        var groupsHeader = GetHeaderValue(request, "X-User-Groups");
        
        if (string.IsNullOrEmpty(groupsHeader))
        {
            _logger.LogWarning("Header X-User-Groups no encontrado");
            return new List<string>();
        }
        
        return groupsHeader.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(g => g.Trim())
                          .ToList();
    }

    /// <summary>
    /// Método helper para extraer valores de headers
    /// </summary>
    private string? GetHeaderValue(HttpRequestData request, string headerName)
    {
        try
        {
            if (request.Headers.TryGetValues(headerName, out var values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer header {HeaderName}", headerName);
            return null;
        }
    }

    /// <summary>
    /// Verifica si el usuario tiene un grupo específico
    /// </summary>
    public bool HasGroup(HttpRequestData request, string groupName)
    {
        var userGroups = GetUserGroups(request);
        return userGroups.Contains(groupName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica si el usuario tiene alguno de los grupos especificados
    /// </summary>
    public bool HasAnyGroup(HttpRequestData request, params string[] groupNames)
    {
        var userGroups = GetUserGroups(request);
        return groupNames.Any(group => userGroups.Contains(group, StringComparer.OrdinalIgnoreCase));
    }
}
