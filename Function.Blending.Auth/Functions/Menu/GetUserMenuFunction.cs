using System;
using System.Net;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Menu.Queries;
using Function.Blending.Auth.Application.Menu.DTOs;
using Function.Blending.Auth.Application.Interfaces.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Functions.Menu
{
    /// <summary>
    /// Azure Function responsable del endpoint de menú de usuario.
    /// 
    /// ARQUITECTURA:
    /// - Presentation Layer: Esta Function (HTTP endpoint)
    /// - Application Layer: GetUserMenuQuery + GetUserMenuHandler (CQRS)
    /// - Infrastructure Layer: HeaderUserService (lee headers de APIM)
    /// 
    /// RESPONSABILIDADES:
    /// - Recibir requests HTTP
    /// - Extraer información de usuario desde headers APIM
    /// - Delegar lógica de negocio al Application Layer
    /// - Retornar respuestas HTTP apropiadas
    /// </summary>
    public class GetUserMenuFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserMenuFunction> _logger;
        private readonly IHeaderUserService _headerUserService;
        private readonly IHttpResponseService _responseService;

        public GetUserMenuFunction(
            IMediator mediator, 
            ILogger<GetUserMenuFunction> logger,
            IHeaderUserService headerUserService,
            IHttpResponseService responseService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _headerUserService = headerUserService ?? throw new ArgumentNullException(nameof(headerUserService));
            _responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
        }

        /// <summary>
        /// Endpoint GET para obtener el menú de usuario basado en sus permisos.
        /// 
        /// FLUJO:
        /// 1. APIM valida JWT y agrega headers X-User-*
        /// 2. HeaderUserService extrae información de usuario
        /// 3. Se crea GetUserMenuQuery con datos del usuario
        /// 4. MediatR envía query al GetUserMenuHandler
        /// 5. Handler aplica lógica de negocio y retorna resultado
        /// 6. Function convierte resultado a HTTP response
        /// </summary>
        [Function(FunctionNames.User.GetMenu)]
        public async Task<HttpResponseData> GetUserMenu(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Auth.User.Menu)] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation($"Iniciando procesamiento de solicitud de menú de usuario ${req.Headers}");

                // PASO 1: Extraer información de usuario desde headers agregados por APIM
                // HeaderUserService lee X-User-Id, X-User-Name, X-User-Groups, etc.
                var userId = _headerUserService.GetCurrentUserId(req);
                var userName = _headerUserService.GetCurrentUserName(req);
                var userGroups = _headerUserService.GetUserGroups(req);

                // PASO 2: Validar que la información del usuario esté disponible
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Headers de usuario faltantes o inválidos - Usuario no autenticado");
                    return await _responseService.CreateErrorResponseAsync(
                        req, 
                        "Información de usuario no disponible", 
                        HttpStatusCode.Unauthorized);
                }

                _logger.LogInformation("Usuario autenticado correctamente: {UserId} - {UserName} - Grupos: {Groups}", 
                    userId, userName, string.Join(",", userGroups));

                // PASO 3: Crear query CQRS con información del usuario
                // Esta es la entrada al Application Layer siguiendo Clean Architecture
                var query = new GetUserMenuQuery(userId.ToString(), userName, userGroups);
                
                // PASO 4: Enviar query usando MediatR al Application Layer
                // GetUserMenuHandler procesará la lógica de negocio
                var result = await _mediator.Send(query);

                // PASO 5: Procesar resultado y generar respuesta HTTP apropiada
                if (!result.Success)
                {
                    _logger.LogWarning("Error procesando menú de usuario: {Message}", result.Message);
                    return await _responseService.CreateErrorResponseAsync(
                        req, 
                        result.Message ?? "Error procesando menú de usuario", 
                        (HttpStatusCode)result.StatusCode);
                }

                _logger.LogInformation("Menú de usuario generado exitosamente para {UserId}", userId);
                return await _responseService.CreateSuccessResponseAsync(
                    req, 
                    result.Data ?? new MenuData(), 
                    result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno procesando solicitud de menú de usuario");
                return await _responseService.CreateErrorResponseAsync(
                    req, 
                    "Error interno del servidor", 
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}
