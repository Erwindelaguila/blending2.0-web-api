using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Function.Blending.Core.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de autorización que lee y valida tokens JWT
/// directamente desde el header Authorization.
/// 
/// Este servicio decodifica tokens JWT para extraer información del usuario
/// como ID, nombre y scopes/permisos sin depender de simuladores externos.
/// 
/// NOTA: Para Azure Functions Workers, utilizamos AsyncLocal para mantener
/// el contexto de token a través de llamadas async sin depender del thread ID.
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthorizationService> _logger;
    private readonly IAuthorizationHeaderExtractor _headerExtractor;
    private readonly ITokenClaimExtractor _tokenExtractor;
    
    // Cache thread-safe para token JWT usando AsyncLocal para mantener contexto
    private static readonly AsyncLocal<string?> _currentJwtToken = new();
    private static readonly AsyncLocal<HttpRequestData?> _currentRequestData = new();
    
    // MÉTODOS DE COMPATIBILIDAD TEMPORAL - Para funciones que aún no se han migrado
    private static readonly AsyncLocal<Dictionary<string, string>?> _currentRequestHeaders = new();

    public AuthorizationService(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<AuthorizationService> logger,
        IAuthorizationHeaderExtractor headerExtractor,
        ITokenClaimExtractor tokenExtractor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _headerExtractor = headerExtractor ?? throw new ArgumentNullException(nameof(headerExtractor));
        _tokenExtractor = tokenExtractor ?? throw new ArgumentNullException(nameof(tokenExtractor));
    }

    /// <summary>
    /// Establece el JWT token para el contexto actual (llamado desde las Functions).
    /// Usa AsyncLocal para mantener el contexto a través de await calls.
    /// </summary>
    public static void SetCurrentJwtToken(string jwtToken)
    {
        _currentJwtToken.Value = jwtToken;
    }

    /// <summary>
    /// Establece el HttpRequestData para el contexto actual (llamado desde las Functions).
    /// </summary>
    public static void SetCurrentRequestData(HttpRequestData requestData)
    {
        _currentRequestData.Value = requestData;
    }

    /// <summary>
    /// Limpia el token del contexto actual.
    /// </summary>
    public static void ClearCurrentContext()
    {
        _currentJwtToken.Value = null;
        _currentRequestData.Value = null;
        _currentRequestHeaders.Value = null; // Compatibilidad
    }

    // ======= MÉTODOS DE COMPATIBILIDAD TEMPORAL =======
    // Estos métodos son para funciones que aún no se han migrado al nuevo sistema JWT
    
    /// <summary>
    /// MÉTODO DE COMPATIBILIDAD: Para funciones que aún no se han migrado
    /// </summary>
    public static void SetCurrentRequestHeaders(Dictionary<string, string> headers)
    {
        _currentRequestHeaders.Value = headers;
    }

    /// <summary>
    /// MÉTODO DE COMPATIBILIDAD: Para funciones que aún no se han migrado
    /// </summary>
    public static void ClearCurrentRequestHeaders()
    {
        _currentRequestHeaders.Value = null;
    }

    /// <summary>
    /// Valida si el usuario actual tiene el scope requerido.
    /// Lee el token JWT y extrae los scopes del claim 'scp', o usa headers de compatibilidad.
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
    /// Obtiene el User ID desde el token JWT (claims oid o sub), o headers de compatibilidad.
    /// </summary>
    public string GetCurrentUserId()
    {
        // Intentar primero con JWT
        var jwtToken = GetCurrentJwtToken();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            var userId = _tokenExtractor.GetUserObjectId(jwtToken);
            if (!string.IsNullOrEmpty(userId)) return userId;
        }
        
        // Fallback a headers de compatibilidad
        var headers = _currentRequestHeaders.Value;
        if (headers != null && headers.TryGetValue("X-User-Id", out var userIdHeader) && !string.IsNullOrEmpty(userIdHeader))
        {
            return userIdHeader;
        }

        _logger.LogWarning("No se pudo obtener User ID, retornando usuario anónimo");
        return "anonymous";
    }

    /// <summary>
    /// Obtiene el nombre del usuario desde el token JWT o headers de compatibilidad.
    /// </summary>
    public string GetCurrentUserName()
    {
        // Intentar primero con JWT
        var jwtToken = GetCurrentJwtToken();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            var userName = _tokenExtractor.GetUserName(jwtToken);
            if (!string.IsNullOrEmpty(userName)) return userName;
        }
        
        // Fallback a headers de compatibilidad
        var headers = _currentRequestHeaders.Value;
        if (headers != null && headers.TryGetValue("X-User-Name", out var userNameHeader) && !string.IsNullOrEmpty(userNameHeader))
        {
            return userNameHeader;
        }

        return "Unknown User";
    }

    /// <summary>
    /// Obtiene todos los scopes del usuario desde el token JWT o headers de compatibilidad.
    /// </summary>
    public string[] GetCurrentUserScopes()
    {
        // Intentar primero con JWT
        var jwtToken = GetCurrentJwtToken();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            var scopes = _tokenExtractor.GetUserScopes(jwtToken);
            if (scopes != null && scopes.Any()) return scopes.ToArray();
        }
        
        // Fallback a headers de compatibilidad
        var headers = _currentRequestHeaders.Value;
        if (headers != null && headers.TryGetValue("X-User-Scopes", out var scopesHeader) && !string.IsNullOrEmpty(scopesHeader))
        {
            return scopesHeader.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        _logger.LogWarning("No se pudieron obtener scopes, retornando scopes vacíos");
        return Array.Empty<string>();
    }

    /// <summary>
    /// Obtiene el token JWT del contexto actual.
    /// Intenta primero desde AsyncLocal, luego extrae del HttpRequestData o HttpContext.
    /// </summary>
    private string? GetCurrentJwtToken()
    {
        // Intentar primero desde el AsyncLocal context
        var cachedToken = _currentJwtToken.Value;
        if (!string.IsNullOrEmpty(cachedToken))
        {
            _logger.LogDebug("Token JWT obtenido desde AsyncLocal context");
            return cachedToken;
        }

        // Intentar desde HttpRequestData
        var requestData = _currentRequestData.Value;
        if (requestData != null)
        {
            _logger.LogDebug("Extrayendo token JWT desde HttpRequestData");
            var token = _headerExtractor.ExtractJwtToken(requestData);
            if (!string.IsNullOrEmpty(token))
            {
                // Cache para siguientes llamadas
                _currentJwtToken.Value = token;
                return token;
            }
        }

        // Fallback a HttpContext (para casos donde esté disponible)
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            _logger.LogDebug("Extrayendo token JWT desde HttpContext");
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                // Cache para siguientes llamadas
                _currentJwtToken.Value = token;
                return token;
            }
        }

        _logger.LogWarning("No se pudo obtener token JWT desde ningún contexto");
        return null;
    }
}
