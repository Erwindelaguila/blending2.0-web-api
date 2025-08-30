using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Function.Blending.Core.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de autorización que lee headers X-User-*
/// generados por el APIM Simulator (o APIM real en producción).
/// 
/// Este servicio replica exactamente el comportamiento de autorización
/// que tendríamos con Azure APIM en producción.
/// 
/// NOTA: Para Azure Functions Workers, utilizamos AsyncLocal para mantener
/// el contexto de headers a través de llamadas async sin depender del thread ID.
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthorizationService> _logger;
    
    // Cache thread-safe para headers de Azure Functions usando AsyncLocal para mantener contexto
    private static readonly AsyncLocal<Dictionary<string, string>?> _currentRequestHeaders = new();

    public AuthorizationService(IHttpContextAccessor httpContextAccessor, ILogger<AuthorizationService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Establece los headers para el contexto actual (llamado desde las Functions).
    /// Usa AsyncLocal para mantener el contexto a través de await calls.
    /// </summary>
    public static void SetCurrentRequestHeaders(Dictionary<string, string> headers)
    {
        _currentRequestHeaders.Value = headers;
    }

    /// <summary>
    /// Limpia los headers del contexto actual.
    /// </summary>
    public static void ClearCurrentRequestHeaders()
    {
        _currentRequestHeaders.Value = null;
    }

    /// <summary>
    /// Valida si el usuario actual tiene el scope requerido.
    /// Lee el header X-User-Scopes generado por APIM/APIM Simulator.
    /// </summary>
    public bool HasRequiredScope(string requiredScope)
    {
        try
        {
            var userScopes = GetCurrentUserScopes();
            var hasScope = userScopes.Contains(requiredScope, StringComparer.OrdinalIgnoreCase);
            
            _logger.LogDebug("VALIDACION SCOPE:");
            _logger.LogDebug("  Scope requerido: {RequiredScope}", requiredScope);
            _logger.LogDebug("  Scopes del usuario: {UserScopes}", string.Join(", ", userScopes));
            _logger.LogDebug("  Resultado: {HasScope}", hasScope ? "PERMITIDO" : "DENEGADO");
            
            return hasScope;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando scope {RequiredScope}", requiredScope);
            return false;
        }
    }

    /// <summary>
    /// Obtiene el User ID desde el header X-User-Id.
    /// </summary>
    public string GetCurrentUserId()
    {
        var headers = GetCurrentHeaders();
        if (headers == null)
        {
            _logger.LogWarning("No se pudieron obtener headers, retornando usuario anónimo");
            return "anonymous";
        }

        return headers.TryGetValue("X-User-Id", out var userId) && !string.IsNullOrEmpty(userId) 
            ? userId 
            : "anonymous";
    }

    /// <summary>
    /// Obtiene el nombre del usuario desde el header X-User-Name.
    /// </summary>
    public string GetCurrentUserName()
    {
        var headers = GetCurrentHeaders();
        if (headers == null) return "Unknown User";

        return headers.TryGetValue("X-User-Name", out var userName) && !string.IsNullOrEmpty(userName) 
            ? userName 
            : "Unknown User";
    }

    /// <summary>
    /// Obtiene todos los scopes del usuario desde el header X-User-Scopes.
    /// </summary>
    public string[] GetCurrentUserScopes()
    {
        var headers = GetCurrentHeaders();
        if (headers == null)
        {
            _logger.LogWarning("No se pudieron obtener headers, retornando scopes vacíos");
            return Array.Empty<string>();
        }

        if (!headers.TryGetValue("X-User-Scopes", out var scopesHeader) || string.IsNullOrEmpty(scopesHeader))
        {
            _logger.LogWarning("Header X-User-Scopes no encontrado o vacío");
            return Array.Empty<string>();
        }

        // Los scopes vienen separados por espacios (estándar OAuth2)
        return scopesHeader.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Obtiene los headers del contexto actual.
    /// Intenta primero desde AsyncLocal context, luego desde HttpContext.
    /// </summary>
    private Dictionary<string, string>? GetCurrentHeaders()
    {
        // Intentar primero desde el AsyncLocal context
        var cachedHeaders = _currentRequestHeaders.Value;
        if (cachedHeaders != null)
        {
            _logger.LogDebug("Headers obtenidos desde AsyncLocal context");
            return cachedHeaders;
        }

        // Fallback a HttpContext (para casos donde esté disponible)
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            _logger.LogDebug("Headers obtenidos desde HttpContext");
            var headers = new Dictionary<string, string>();
            foreach (var header in context.Request.Headers)
            {
                headers[header.Key] = header.Value.FirstOrDefault() ?? "";
            }
            return headers;
        }

        _logger.LogWarning("No se pudo obtener contexto (ni AsyncLocal context ni HttpContext)");
        return null;
    }
}
