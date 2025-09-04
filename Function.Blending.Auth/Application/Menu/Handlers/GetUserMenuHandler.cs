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
    /// <summary>
    /// Handler simplificado - ya no valida JWT porque APIM lo hizo
    /// Solo procesa lógica de negocio para generar menú
    /// </summary>
    public class GetUserMenuHandler : IRequestHandler<GetUserMenuQuery, MenuResponse>
    {
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly ILogger<GetUserMenuHandler> _logger;

        public GetUserMenuHandler(
            IRoleService roleService,
            IMenuService menuService,
            ILogger<GetUserMenuHandler> logger)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MenuResponse> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Procesando menú para usuario: {UserId} ({UserName}) con grupos: {Groups}", 
                    request.UserId, request.UserName, string.Join(",", request.UserGroups));

                // ✅ Ya no validamos JWT - APIM lo hizo
                // ✅ Información viene directamente de headers

                // Obtener roles del usuario basado en sus grupos
                var userRoles = await _roleService.GetUserRolesAsync(request.UserGroups);
                
                // Generar menú basado en roles
                var menuData = await _menuService.BuildUserMenuAsync(request.UserId, request.UserName, userRoles);

                return MenuResponse.CreateSuccess(menuData, "Menú obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar menú de usuario: {UserId}", request.UserId);
                return MenuResponse.CreateFailure("Error interno del servidor", HttpStatusCode.InternalServerError);
            }
        }
    }
}
