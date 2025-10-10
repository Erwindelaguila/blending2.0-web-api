using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Common;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Menu.DTOs;
using Function.Blending.Auth.Application.Menu.Queries;
using Function.Blending.Auth.Application.Common.Results;
using Function.Blending.Auth.Infrastructure.Middleware;
using MediatR;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Application.Menu.Handlers
{
    // El handler devuelve HttpResponseData directamente, para integrarse bien con Azure Functions
    public class GetUserMenuHandler : IRequestHandler<GetUserMenuQuery, HttpResponseData>
    {
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IHttpResponseService _httpResponseService;
        private readonly ILogger<GetUserMenuHandler> _logger;

        public GetUserMenuHandler(
            IRoleService roleService,
            IMenuService menuService,
            IHttpResponseService httpResponseService,
            ILogger<GetUserMenuHandler> logger)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _httpResponseService = httpResponseService ?? throw new ArgumentNullException(nameof(httpResponseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HttpResponseData> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
        {
            var traceId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("TRACE_ID: {TraceId} | Iniciando construcción del menú para el usuario {UserId}", 
                    traceId, request.UserClaims.ObjectId);

                var validationResult = ValidateUserClaims(request.UserClaims);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("TRACE_ID: {TraceId} | Validación fallida para usuario {UserId}: {ValidationError}", 
                        traceId, request.UserClaims.ObjectId, validationResult.ErrorMessage);
                    
                    return await _httpResponseService.CreateErrorResponseAsync(
                        request.HttpRequest,
                        validationResult.ErrorMessage,
                        HttpStatusCode.Forbidden);
                }

                var userRoles = await _roleService.GetUserRolesAsync(request.UserClaims.Groups);
                var menuData = await _menuService.BuildUserMenuAsync(
                    request.UserClaims.ObjectId!,
                    request.UserClaims.Name ?? "Usuario",
                    userRoles);

                _logger.LogInformation("Menú construido exitosamente para el usuario {UserId}", request.UserClaims.ObjectId);
                return await _httpResponseService.CreateSuccessResponseAsync(request.HttpRequest, menuData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al construir el menú para el usuario {UserId}", request.UserClaims.ObjectId);
                return await _httpResponseService.CreateErrorResponseAsync(
                    request.HttpRequest,
                    "Error interno al construir el menú",
                    HttpStatusCode.InternalServerError);
            }
        }

        private static (bool IsValid, string ErrorMessage) ValidateUserClaims(UserClaims userClaims)
        {
            if (string.IsNullOrEmpty(userClaims.ObjectId))
                return (false, "Object ID del usuario es requerido");
            
            if (userClaims.Groups.Count == 0)
                return (false, "Grupos del usuario son requeridos");
                
            return (true, string.Empty);
        }
    }
}
