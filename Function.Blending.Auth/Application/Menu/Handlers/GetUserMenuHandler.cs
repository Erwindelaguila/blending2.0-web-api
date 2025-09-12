using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Menu.DTOs;
using Function.Blending.Auth.Application.Menu.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Application.Menu.Handlers
{
    public class GetUserMenuHandler : IRequestHandler<GetUserMenuQuery, MenuResponse>
    {
        private readonly ITokenService _tokenService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly ILogger<GetUserMenuHandler> _logger;

        public GetUserMenuHandler(
            ITokenService tokenService,
            IRoleService roleService,
            IMenuService menuService,
            ILogger<GetUserMenuHandler> logger)
        {
            _tokenService = tokenService;
            _roleService = roleService;
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<MenuResponse> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
        {
            if (!await _tokenService.ValidateTokenAsync(request.JwtToken))
            {
                return MenuResponse.CreateFailure("Token JWT inválido", HttpStatusCode.Unauthorized);
            }

            try
            {
                var userInfo = ExtractUserInfoFromToken(request.JwtToken);
                if (!IsValidUserInfo(userInfo))
                {
                    return MenuResponse.CreateFailure("Token no contiene información suficiente del usuario", HttpStatusCode.Forbidden);
                }

                var userRoles = await _roleService.GetUserRolesAsync(userInfo.Groups);
                var menuData = await _menuService.BuildUserMenuAsync(userInfo.Id, userInfo.Name, userRoles);

                return MenuResponse.CreateSuccess(menuData, "Menú obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar menú de usuario");
                return MenuResponse.CreateFailure("Error interno del servidor", HttpStatusCode.InternalServerError);
            }
        }

        private (string Id, string Name, List<string> Groups) ExtractUserInfoFromToken(string jwtToken)
        {
            return (
                Id: _tokenService.GetUserObjectId(jwtToken) ?? string.Empty,
                Name: _tokenService.GetUserName(jwtToken) ?? "Usuario",
                Groups: _tokenService.GetUserGroups(jwtToken) ?? new List<string>()
            );
        }

        private static bool IsValidUserInfo((string Id, string Name, List<string> Groups) userInfo)
        {
            return !string.IsNullOrEmpty(userInfo.Id) && userInfo.Groups.Any();
        }
    }
}
