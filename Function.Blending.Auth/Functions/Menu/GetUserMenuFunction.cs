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
    public class GetUserMenuFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserMenuFunction> _logger;
        private readonly IAuthorizationHeaderExtractor _authExtractor;
        private readonly IHttpResponseService _responseService;

        public GetUserMenuFunction(
            IMediator mediator, 
            ILogger<GetUserMenuFunction> logger,
            IAuthorizationHeaderExtractor authExtractor,
            IHttpResponseService responseService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authExtractor = authExtractor ?? throw new ArgumentNullException(nameof(authExtractor));
            _responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
        }

        [Function(FunctionNames.User.GetMenu)]
        public async Task<HttpResponseData> GetUserMenu(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Auth.User.Menu)] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("Iniciando solicitud de menú de usuario");

                // Extraer JWT token del header Authorization
                var jwtToken = _authExtractor.ExtractJwtToken(req);
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return await _responseService.CreateErrorResponseAsync(req, "Token de autorización requerido", HttpStatusCode.Unauthorized);
                }

                // Procesar la consulta usando CQRS
                var query = new GetUserMenuQuery(jwtToken);
                var result = await _mediator.Send(query);

                // Generar respuesta basada en el resultado
                if (!result.Success)
                {
                    _logger.LogWarning("Error al procesar menú: {Message}", result.Message);
                    return await _responseService.CreateErrorResponseAsync(req, result.Message ?? "Error al procesar menú", (HttpStatusCode)result.StatusCode);
                }

                _logger.LogInformation("Menú de usuario obtenido exitosamente");
                return await _responseService.CreateSuccessResponseAsync(req, result.Data ?? new MenuData(), result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al procesar solicitud de menú");
                return await _responseService.CreateErrorResponseAsync(req, "Error interno del servidor", HttpStatusCode.InternalServerError);
            }
        }
    }
}
