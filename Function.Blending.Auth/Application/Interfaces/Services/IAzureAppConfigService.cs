using Function.Blending.Auth.Application.Menu.DTOs;

namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface IAzureAppConfigService
    {
        Task<AzureAppConfigModels.AzureGroupMapping> GetAzureGroupMappingAsync();
        Task<AzureAppConfigModels.EnlacesConfiguration> GetEnlacesConfigurationAsync();
        Task<AzureAppConfigModels.NavigationPermisos> GetNavigationPermisosAsync();
    }
}
