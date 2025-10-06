using System;
using System.Net;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Menu.Queries;
using Function.Blending.Auth.Infrastructure.Extensions;
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

        public GetUserMenuFunction(
            IMediator mediator,
            ILogger<GetUserMenuFunction> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function(FunctionNames.User.GetMenu)]
        public async Task<HttpResponseData> GetUserMenu(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Auth.User.Menu)] HttpRequestData req,
            FunctionContext context)
        {
            try
            {
                _logger.LogInformation("🍔 GetUserMenu: Iniciando procesamiento de solicitud de menú");
                
                var userClaims = context.GetUserClaims();
                if (userClaims == null)
                {
                    _logger.LogWarning("⚠️ GetUserMenu: No se encontraron claims de usuario en el contexto");
                    var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await unauthorizedResponse.WriteStringAsync("No autorizado");
                    return unauthorizedResponse;
                }
                
                _logger.LogInformation("👤 GetUserMenu: Usuario autenticado - {UserId}", userClaims.ObjectId);
                
                var query = new GetUserMenuQuery(userClaims, req);
                var httpResponse = await _mediator.Send(query);
                
                _logger.LogInformation("✅ GetUserMenu: Menú procesado exitosamente");
                return httpResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetUserMenu: Error procesando solicitud de menú");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error interno del servidor: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
