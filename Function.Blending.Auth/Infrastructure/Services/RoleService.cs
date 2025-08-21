using Function.Blending.Auth.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IAzureAppConfigService _appConfigService;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IAzureAppConfigService appConfigService, ILogger<RoleService> logger)
    {
        _appConfigService = appConfigService;
        _logger = logger;
    }

    public async Task<List<string>> GetUserRolesAsync(List<string> userGroups)
    {
        var groupMapping = await _appConfigService.GetAzureGroupMappingAsync();
        var userRoles = new List<string>();

        if (groupMapping.GroupToRoleMap != null)
        {
            foreach (var group in userGroups)
            {
                if (groupMapping.GroupToRoleMap.TryGetValue(group, out var role))
                {
                    if (!userRoles.Contains(role))
                    {
                        userRoles.Add(role);
                    }
                }
            }
        }

        return userRoles;
    }
}
