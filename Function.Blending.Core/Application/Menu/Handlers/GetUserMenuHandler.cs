using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Menu.DTOs;
using Function.Blending.Core.Application.Menu.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Menu.Handlers
{
    public class GetUserMenuHandler : IRequestHandler<GetUserMenuQuery, MenuResponse>
    {
        private readonly IAzureAppConfigService _appConfigService;
        private readonly ITokenService _tokenService;
        private readonly IGraphService _graphService;
        private readonly ILogger<GetUserMenuHandler> _logger;

        public GetUserMenuHandler(
            IAzureAppConfigService appConfigService,
            ITokenService tokenService,
            IGraphService graphService,
            ILogger<GetUserMenuHandler> logger)
        {
            _appConfigService = appConfigService;
            _tokenService = tokenService;
            _graphService = graphService;
            _logger = logger;
        }

        public async Task<MenuResponse> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
        {
            if (!await _tokenService.ValidateTokenAsync(request.JwtToken))
            {
                _logger.LogWarning("Token JWT inválido"); 
                return MenuResponse.CreateFailure("Token JWT inválido", HttpStatusCode.Unauthorized);
            }

            try
            {
                _logger.LogInformation("Iniciando proceso de obtención de menú de usuario"); 

     
                GraphUserInfo userInfo;
                List<string> userGroups;

                try
                {
                    _logger.LogInformation("Consultando Microsoft Graph para obtener información del usuario");

                    var userInfoTask = _graphService.GetUserInfoAsync(request.JwtToken);
                    var userGroupsTask = _graphService.GetUserGroupsAsync(request.JwtToken);
                    await Task.WhenAll(userInfoTask, userGroupsTask);
                    (userInfo, userGroups) = (await userInfoTask, await userGroupsTask);

                    _logger.LogInformation("Información obtenida desde Microsoft Graph: Usuario {UserId}, {GroupCount} grupos", userInfo.Id, userGroups.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al consultar Microsoft Graph, usando fallback desde token");

                    var userId = _tokenService.GetUserObjectId(request.JwtToken);
                    var userName = _tokenService.GetUserName(request.JwtToken);
                    var userEmail = _tokenService.GetUserEmail(request.JwtToken);
                    var tokenGroups = _tokenService.GetUserGroups(request.JwtToken);

                    if (string.IsNullOrEmpty(userId) || tokenGroups == null || !tokenGroups.Any())
                    {
                        _logger.LogWarning("Token no contiene información suficiente del usuario y Graph falló");
                        return MenuResponse.CreateFailure("No se pudo obtener información del usuario", HttpStatusCode.Forbidden);
                    }

                    userInfo = new GraphUserInfo
                    {
                        Id = userId,
                        DisplayName = userName ?? "Usuario",
                        Mail = userEmail,
                        UserPrincipalName = userEmail
                    };
                    userGroups = tokenGroups;

                    _logger.LogInformation("Información obtenida desde token como fallback: Usuario {UserId}, {GroupCount} grupos", userInfo.Id, userGroups.Count);
                }

      
                var groupMapping = await _appConfigService.GetAzureGroupMappingAsync();
                _logger.LogInformation("Obtenido mapping de {MappingCount} grupos a roles", groupMapping.GroupToRoleMap?.Count ?? 0);

                var enlacesConfig = await _appConfigService.GetEnlacesConfigurationAsync();
                _logger.LogInformation("Obtenida configuración de {EnlaceCount} enlaces", enlacesConfig.Enlaces?.Count ?? 0);


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
                                _logger.LogDebug("Usuario asignado al rol: {Role} (desde grupo: {Group})", role, group);
                            }
                        }
                    }
                }
                var permissionsConfig = await _appConfigService.GetNavigationPermisosAsync();
                var permisosUsuario = permissionsConfig.PermisosPorRol
                    ?.Where(p => userRoles.Contains(p.Key))
                    .SelectMany(p => p.Value)
                    .Distinct()
                    .ToList() ?? new List<string>();

                _logger.LogInformation("Usuario tiene {RoleCount} roles y {PermissionCount} permisos", userRoles.Count, permisosUsuario.Count);

                var menuData = new MenuData
                {
                    Enlaces = enlacesConfig.Enlaces ?? new Dictionary<string, EnlaceItem>(),
                    PermisosUsuario = permisosUsuario,
                    UserInfo = new UserInfo
                    {
                        Id = userInfo.Id,
                        Name = userInfo.GivenName ?? userInfo.DisplayName,
                        LastName = userInfo.Surname,
                        Email = userInfo.Mail ?? userInfo.UserPrincipalName,
                        Roles = userRoles
                    }
                };

                var response = MenuResponse.CreateSuccess(menuData, "Menú obtenido exitosamente");

                _logger.LogInformation("Menú de usuario construido exitosamente");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar menú de usuario");
                return MenuResponse.CreateFailure("Error interno del servidor", HttpStatusCode.InternalServerError);
            }
        }
    }
}
