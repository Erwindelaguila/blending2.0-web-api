using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Menu.DTOs;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly IAzureAppConfigService _appConfigService;
    private readonly ILogger<MenuService> _logger;

    public MenuService(IAzureAppConfigService appConfigService, ILogger<MenuService> logger)
    {
        _appConfigService = appConfigService;
        _logger = logger;
    }

    public async Task<MenuData> BuildUserMenuAsync(string userId, string userName, List<string> userRoles)
    {
        var enlacesConfig = await _appConfigService.GetEnlacesConfigurationAsync();
        var permissionsConfig = await _appConfigService.GetNavigationPermisosAsync();

        var permisosUsuario = GetUserPermissions(userRoles, permissionsConfig);
        var enlacesFiltrados = FilterUserLinks(permisosUsuario, enlacesConfig);

        return new MenuData
        {
            Enlaces = enlacesFiltrados,
            PermisosUsuario = permisosUsuario,
            UserInfo = new UserInfo
            {
                Id = userId,
                Name = userName,
                Roles = userRoles
            }
        };
    }

    private List<string> GetUserPermissions(List<string> userRoles, AzureAppConfigModels.NavigationPermisos permissionsConfig)
    {
        return permissionsConfig.PermisosPorRol?
            .Where(p => userRoles.Contains(p.Key))
            .SelectMany(p => p.Value)
            .Distinct()
            .ToList() ?? new List<string>();
    }

    private Dictionary<string, EnlaceItem> FilterUserLinks(List<string> permisosUsuario, AzureAppConfigModels.EnlacesConfiguration enlacesConfig)
    {
        var enlacesConPermisoDirecto = enlacesConfig.Enlaces?
            .Where(enlace => permisosUsuario.Contains(enlace.Key))
            .ToList() ?? new List<KeyValuePair<string, EnlaceItem>>();

        var gruposPadreNecesarios = enlacesConPermisoDirecto
            .Where(enlace => !string.IsNullOrEmpty(enlace.Value.Grupo))
            .Select(enlace => enlace.Value.Grupo!)
            .Distinct()
            .ToList();

        var gruposPadre = enlacesConfig.Enlaces?
            .Where(enlace => gruposPadreNecesarios.Contains(enlace.Key) && 
                           !permisosUsuario.Contains(enlace.Key))
            .ToList() ?? new List<KeyValuePair<string, EnlaceItem>>();

        return enlacesConPermisoDirecto
            .Concat(gruposPadre)
            .ToDictionary(enlace => enlace.Key, enlace => enlace.Value);
    }
}
