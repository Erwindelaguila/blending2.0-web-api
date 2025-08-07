using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Menu.DTOs;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Infrastructure.Services
{
   
    public class AzureAppConfigService : IAzureAppConfigService
    {
        private readonly ConfigurationClient _client;
        private readonly ILogger<AzureAppConfigService> _logger;
        
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
                var error = "Azure App Configuration connection string is not configured";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            try
            {
                _client = new ConfigurationClient(connectionString);
                _logger.LogDebug("Azure App Configuration client initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure App Configuration client");
                throw;
            }
        }

        public async Task<AzureAppConfigModels.AzureGroupMapping> GetAzureGroupMappingAsync()
        {
            _logger.LogInformation("Retrieving Azure Group Mapping configuration");
            
            var groupMapping = await GetConfigValueAsync<Dictionary<string, string>>(
                AzureAppConfigKeys.AZURE_GROUP_MAPPING);
            
            var result = new AzureAppConfigModels.AzureGroupMapping
            {
                GroupToRoleMap = groupMapping ?? new Dictionary<string, string>()
            };
            
            _logger.LogDebug("Retrieved {MappingCount} group mappings", result.GroupToRoleMap.Count);
            return result;
        }
        
        public async Task<AzureAppConfigModels.EnlacesConfiguration> GetEnlacesConfigurationAsync()
        {
            _logger.LogInformation("Retrieving Enlaces configuration");
            
            var enlaces = await GetConfigValueAsync<Dictionary<string, EnlaceItem>>(
                AzureAppConfigKeys.ENLACES);
            
            var result = new AzureAppConfigModels.EnlacesConfiguration
            {
                Enlaces = enlaces ?? new Dictionary<string, EnlaceItem>()
            };
            
            _logger.LogDebug("Retrieved {EnlaceCount} enlaces", result.Enlaces.Count);
            return result;
        }

        public async Task<AzureAppConfigModels.NavigationPermisos> GetNavigationPermisosAsync()
        {
            _logger.LogInformation("Retrieving Navigation Permisos configuration");
            
            var permisos = await GetConfigValueAsync<Dictionary<string, List<string>>>(
                AzureAppConfigKeys.NAVIGATION_PERMISOS);
            
            var result = new AzureAppConfigModels.NavigationPermisos
            {
                PermisosPorRol = permisos ?? new Dictionary<string, List<string>>()
            };
            
            _logger.LogDebug("Retrieved permissions for {RoleCount} roles", result.PermisosPorRol.Count);
            return result;
        }

        private async Task<T?> GetConfigValueAsync<T>(string key) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogWarning("Configuration key is null or empty");
                    return default;
                }

                _logger.LogDebug("Retrieving configuration key: {Key}", key);
                
                var setting = await _client.GetConfigurationSettingAsync(key);
                var value = setting?.Value?.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    _logger.LogWarning("Configuration setting '{Key}' is null, empty, or not found", key);
                    return default;
                }

                _logger.LogDebug("Retrieved configuration value for key '{Key}' with length {ValueLength}", 
                    key, value.Length);

                var result = JsonSerializer.Deserialize<T>(value, JsonOptions);
                
                if (result == null)
                {
                    _logger.LogWarning("Deserialization resulted in null for key '{Key}'", key);
                }
                else
                {
                    _logger.LogDebug("Successfully deserialized configuration for key '{Key}' to type {Type}", 
                        key, typeof(T).Name);
                }

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed for configuration key '{Key}'. " +
                    "The stored value may have an invalid format for type {Type}", key, typeof(T).Name);
                return default;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogWarning("Configuration key '{Key}' not found in Azure App Configuration", key);
                return default;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "Azure App Configuration request failed for key '{Key}'. " +
                    "Status: {Status}, Error: {ErrorCode}", key, ex.Status, ex.ErrorCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving configuration key '{Key}'", key);
                return default;
            }
        }
    }
}
