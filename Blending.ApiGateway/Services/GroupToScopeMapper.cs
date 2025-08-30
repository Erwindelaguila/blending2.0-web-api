using Azure.Data.AppConfiguration;

namespace Blending.ApiGateway.Services;

/// <summary>
/// Servicio que mapea grupos de Azure AD a scopes específicos de la aplicación
/// utilizando Azure App Configuration.
/// 
/// INTEGRACIÓN CON SISTEMA EXISTENTE:
/// - Usa la MISMA Azure App Configuration que Function.Blending.Auth
/// - Mantiene compatibilidad con grupos → roles existentes  
/// - Añade nueva funcionalidad grupos → scopes para autorización
/// 
/// Replica exactamente el comportamiento que tendría Azure APIM en producción
/// al leer la configuración de grupos y scopes.
/// </summary>
public interface IGroupToScopeMapper
{
    /// <summary>
    /// Convierte una lista de grupos de Azure AD en scopes específicos de la aplicación.
    /// </summary>
    /// <param name="userGroups">Array de GUIDs de grupos del usuario</param>
    /// <returns>String con scopes separados por espacios (ej: "appparams.read appparams.write")</returns>
    Task<string> MapGroupsToScopesAsync(string[] userGroups);
}

/// <summary>
/// Implementación que lee la configuración de Azure App Configuration
/// para mapear grupos a scopes exactamente como lo haría APIM en producción.
/// </summary>
public class GroupToScopeMapper : IGroupToScopeMapper
{
    private readonly ConfigurationClient _appConfigClient;
    private readonly ILogger<GroupToScopeMapper> _logger;
    private readonly Dictionary<string, string[]> _groupScopeCache;

    public GroupToScopeMapper(ConfigurationClient appConfigClient, ILogger<GroupToScopeMapper> logger)
    {
        _appConfigClient = appConfigClient ?? throw new ArgumentNullException(nameof(appConfigClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _groupScopeCache = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Mapea grupos de Azure AD a scopes específicos usando App Configuration.
    /// </summary>
    public async Task<string> MapGroupsToScopesAsync(string[] userGroups)
    {
        if (userGroups == null || userGroups.Length == 0)
        {
            _logger.LogInformation("Usuario sin grupos asignados - sin scopes");
            return "";
        }

        _logger.LogInformation("Mapeando {GroupCount} grupos a scopes", userGroups.Length);
        
        var allScopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var groupId in userGroups)
        {
            try
            {
                var scopes = await GetScopesForGroupAsync(groupId);
                foreach (var scope in scopes)
                {
                    allScopes.Add(scope);
                }
                
                _logger.LogDebug("Grupo {GroupId} → scopes: {Scopes}", 
                    groupId, string.Join(", ", scopes));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error obteniendo scopes para grupo {GroupId}", groupId);
            }
        }

        var result = string.Join(" ", allScopes.OrderBy(s => s));
        _logger.LogInformation("Scopes finales: '{FinalScopes}'", result);
        
        return result;
    }

    /// <summary>
    /// Obtiene los scopes para un grupo específico desde App Configuration.
    /// Usa cache para evitar múltiples llamadas a App Config.
    /// </summary>
    private async Task<string[]> GetScopesForGroupAsync(string groupId)
    {
        // Verificar cache primero
        if (_groupScopeCache.TryGetValue(groupId, out var cachedScopes))
        {
            return cachedScopes;
        }

        try
        {
            // Buscar en App Configuration con el patrón: Groups:{GroupId}:Scopes
            var configKey = $"Groups:{groupId}:Scopes";
            var response = await _appConfigClient.GetConfigurationSettingAsync(configKey);
            
            if (response?.Value?.Value != null)
            {
                var scopes = response.Value.Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                // Cachear el resultado
                _groupScopeCache[groupId] = scopes;
                
                _logger.LogDebug("App Config: {ConfigKey} = {Scopes}", configKey, string.Join(", ", scopes));
                return scopes;
            }
            else
            {
                _logger.LogWarning("No se encontró configuración para grupo {GroupId} en App Config", groupId);
                
                // Cachear resultado vacío para evitar futuras consultas
                _groupScopeCache[groupId] = Array.Empty<string>();
                return Array.Empty<string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando App Configuration para grupo {GroupId}", groupId);
            
            // En caso de error, cachear resultado vacío
            _groupScopeCache[groupId] = Array.Empty<string>();
            return Array.Empty<string>();
        }
    }
}

/// <summary>
/// Mock implementation para desarrollo cuando no hay App Configuration disponible.
/// Simula el mapeo de grupos conocidos a scopes para testing.
/// </summary>
public class MockGroupToScopeMapper : IGroupToScopeMapper
{
    private readonly ILogger<MockGroupToScopeMapper> _logger;
    private readonly Dictionary<string, string[]> _mockGroupMappings;

    public MockGroupToScopeMapper(ILogger<MockGroupToScopeMapper> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Mapeo mock basado en grupos típicos de la aplicación
        _mockGroupMappings = new Dictionary<string, string[]>
        {
            // Grupo Administradores - todos los permisos
            ["50fd044e-9a2c-45a5-b93a-0129d1d97a41"] = new[] 
            {
                "appparams.read", "appparams.write",
                "plantas.read", "plantas.write",
                "calidad.read", "calidad.write",
                "agregados.read", "agregados.write",
                "parametros.read", "parametros.write",
                "productos.read", "productos.write",
                "tipoproduccion.read", "tipoproduccion.write",
                "lineaproduccion.read", "lineaproduccion.write"
            },
            
            // Grupo Operadores - solo lectura y escritura limitada
            ["dfa05051-c3d5-4eea-a2ed-0051eba9bda0"] = new[]
            {
                "plantas.read", "calidad.read", "parametros.read",
                "productos.read", "tipoproduccion.read", "lineaproduccion.read"
            },
            
            // Grupo Supervisores - lectura completa + algunas escrituras
            ["98b8eed2-ffcd-40fd-9979-39ee06253edf"] = new[]
            {
                "appparams.read", "plantas.read", "plantas.write",
                "calidad.read", "calidad.write", "parametros.read"
            }
        };
    }

    public Task<string> MapGroupsToScopesAsync(string[] userGroups)
    {
        if (userGroups == null || userGroups.Length == 0)
        {
            _logger.LogInformation("🛠️ MOCK: Usuario sin grupos - sin scopes");
            return Task.FromResult("");
        }

        _logger.LogInformation("🛠️ MOCK: Mapeando {GroupCount} grupos a scopes", userGroups.Length);
        
        var allScopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var groupId in userGroups)
        {
            if (_mockGroupMappings.TryGetValue(groupId, out var scopes))
            {
                foreach (var scope in scopes)
                {
                    allScopes.Add(scope);
                }
                
                _logger.LogDebug("🛠️ MOCK: Grupo {GroupId} → {ScopeCount} scopes", 
                    groupId, scopes.Length);
            }
            else
            {
                _logger.LogWarning("🛠️ MOCK: Grupo desconocido {GroupId}", groupId);
            }
        }

        var result = string.Join(" ", allScopes.OrderBy(s => s));
        _logger.LogInformation("🛠️ MOCK: Scopes finales: '{FinalScopes}'", result);
        
        return Task.FromResult(result);
    }
}

/// <summary>
/// Implementación que lee la configuración local desde appsettings.json
/// pero comportándose exactamente como Azure App Configuration.
/// Ideal para desarrollo con configuración real pero sin Azure App Config.
/// </summary>
public class LocalConfigGroupToScopeMapper : IGroupToScopeMapper
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalConfigGroupToScopeMapper> _logger;
    private readonly Dictionary<string, string[]> _groupScopeCache;

    public LocalConfigGroupToScopeMapper(IConfiguration configuration, ILogger<LocalConfigGroupToScopeMapper> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _groupScopeCache = new Dictionary<string, string[]>();
    }

    public Task<string> MapGroupsToScopesAsync(string[] userGroups)
    {
        if (userGroups == null || userGroups.Length == 0)
        {
            _logger.LogInformation("🔧 CONFIG: Usuario sin grupos asignados - sin scopes");
            return Task.FromResult("");
        }

        _logger.LogInformation("🔧 CONFIG: Mapeando {GroupCount} grupos a scopes", userGroups.Length);
        
        var allScopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var groupId in userGroups)
        {
            try
            {
                var scopes = GetScopesForGroup(groupId);
                foreach (var scope in scopes)
                {
                    allScopes.Add(scope);
                }
                
                _logger.LogDebug("🔧 CONFIG: Grupo {GroupId} → scopes: {Scopes}", 
                    groupId, string.Join(", ", scopes));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "🔧 CONFIG: Error obteniendo scopes para grupo {GroupId}", groupId);
            }
        }

        var result = string.Join(" ", allScopes.OrderBy(s => s));
        _logger.LogInformation("🔧 CONFIG: Scopes finales: '{FinalScopes}'", result);
        
        return Task.FromResult(result);
    }

    /// <summary>
    /// Obtiene los scopes para un grupo desde la configuración local.
    /// Simula exactamente el patrón Groups:{GroupId}:Scopes de Azure App Configuration.
    /// </summary>
    private string[] GetScopesForGroup(string groupId)
    {
        // Verificar cache primero
        if (_groupScopeCache.TryGetValue(groupId, out var cachedScopes))
        {
            return cachedScopes;
        }

        try
        {
            // Buscar en configuración local con el patrón: Groups:{GroupId}:Scopes
            var configKey = $"Groups:{groupId}:Scopes";
            var scopesValue = _configuration[configKey];
            
            if (!string.IsNullOrEmpty(scopesValue))
            {
                var scopes = scopesValue
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                // Cachear el resultado
                _groupScopeCache[groupId] = scopes;
                
                _logger.LogDebug("🔧 CONFIG: {ConfigKey} = {Scopes}", configKey, string.Join(", ", scopes));
                return scopes;
            }
            else
            {
                _logger.LogWarning("🔧 CONFIG: No se encontró configuración para grupo {GroupId}", groupId);
                
                // Cachear resultado vacío para evitar futuras consultas
                _groupScopeCache[groupId] = Array.Empty<string>();
                return Array.Empty<string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔧 CONFIG: Error consultando configuración para grupo {GroupId}", groupId);
            
            // En caso de error, cachear resultado vacío
            _groupScopeCache[groupId] = Array.Empty<string>();
            return Array.Empty<string>();
        }
    }
}
