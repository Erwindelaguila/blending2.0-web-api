using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Menu.DTOs;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Services
{

    public class AzureAppConfigService : IAzureAppConfigService
    {
        private readonly ConfigurationClient _client;
        private readonly ILogger<AzureAppConfigService> _logger;
        
        private static readonly ConcurrentDictionary<string, (object Value, DateTime ExpiresAt)> _cache = new();
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
        
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public AzureAppConfigService(ILogger<AzureAppConfigService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var connectionString = Environment.GetEnvironmentVariable("AzureAppConfigConnectionString");
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var error = "La cadena de conexión de Azure App Configuration no está configurada";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            try
            {
                _client = new ConfigurationClient(connectionString);
                _logger.LogInformation("Cliente de Azure App Configuration inicializado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar el cliente de Azure App Configuration");
                throw;
            }
        }

        public async Task<AzureAppConfigModels.AzureGroupMapping> GetAzureGroupMappingAsync()
        {
            try 
            {
                var groupMapping = await GetConfigValueAsync<Dictionary<string, string>>(
                    AzureAppConfigKeys.AZURE_GROUP_MAPPING);
                
                var result = new AzureAppConfigModels.AzureGroupMapping
                {
                    GroupToRoleMap = groupMapping ?? new Dictionary<string, string>()
                };
                
                _logger.LogInformation("Configuración de mapeo de grupos obtenida correctamente: {MappingCount} mapeos", 
                    result.GroupToRoleMap.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la configuración de mapeo de grupos de Azure");
                throw;
            }
        }
        
        public async Task<AzureAppConfigModels.EnlacesConfiguration> GetEnlacesConfigurationAsync()
        {
            try 
            {
                var enlaces = await GetConfigValueAsync<Dictionary<string, EnlaceItem>>(
                    AzureAppConfigKeys.ENLACES);
                
                var result = new AzureAppConfigModels.EnlacesConfiguration
                {
                    Enlaces = enlaces ?? new Dictionary<string, EnlaceItem>()
                };
                
                _logger.LogInformation("Configuración de enlaces obtenida correctamente: {EnlaceCount} enlaces", 
                    result.Enlaces.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la configuración de enlaces");
                throw;
            }
        }

        public async Task<AzureAppConfigModels.NavigationPermisos> GetNavigationPermisosAsync()
        {
            try 
            {
                var permisos = await GetConfigValueAsync<Dictionary<string, List<string>>>(
                    AzureAppConfigKeys.NAVIGATION_PERMISOS);
                
                var result = new AzureAppConfigModels.NavigationPermisos
                {
                    PermisosPorRol = permisos ?? new Dictionary<string, List<string>>()
                };
                
                _logger.LogInformation("Configuración de permisos de navegación obtenida correctamente: {RoleCount} roles", 
                    result.PermisosPorRol.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la configuración de permisos de navegación");
                throw;
            }
        }

        private async Task<T?> GetConfigValueAsync<T>(string key) where T : class
        {
            // Verificar caché primero
            if (_cache.TryGetValue(key, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
            {
                return (T?)cached.Value;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogWarning("La clave de configuración está vacía o es nula");
                    return default;
                }

                var setting = await _client.GetConfigurationSettingAsync(key);
                var value = setting?.Value?.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    _logger.LogWarning("La configuración '{Key}' no se encontró o está vacía", key);
                    return default;
                }

                var result = JsonSerializer.Deserialize<T>(value, JsonOptions);
                
                if (result != null)
                {
                    // Guardar en caché
                    _cache[key] = (result, DateTime.UtcNow.Add(CacheTtl));
                }

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error de deserialización JSON para la configuración '{Key}'", key);
                return default;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 429)
            {
                _logger.LogWarning("Límite de velocidad de Azure App Configuration excedido para '{Key}'", key);
                return default;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogWarning("Configuración '{Key}' no encontrada en Azure App Configuration", key);
                return default;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "Error en la solicitud a Azure App Configuration para '{Key}'. Estado: {Status}", key, ex.Status);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener la configuración '{Key}'", key);
                return default;
            }
        }
    }
}