using Function.Blending.Core.Application.Menu.DTOs;

namespace Function.Blending.Core.Application.Interfaces.Services
{
    public interface IAzureAppConfigService
    {
        Task<AzureAppConfigModels.AzureGroupMapping> GetAzureGroupMappingAsync();
        Task<AzureAppConfigModels.EnlacesConfiguration> GetEnlacesConfigurationAsync();
        Task<AzureAppConfigModels.NavigationPermisos> GetNavigationPermisosAsync();
    }
}
