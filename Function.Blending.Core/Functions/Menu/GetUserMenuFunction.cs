using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Menu.Queries;
using Function.Blending.Core.Application.Menu.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.Menu
{
    public class GetUserMenuFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserMenuFunction> _logger;

        public GetUserMenuFunction(IMediator mediator, ILogger<GetUserMenuFunction> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function(FunctionNames.User.GetMenu)]
        public async Task<HttpResponseData> GetUserMenu(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.User.Menu)] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("Iniciando solicitud de menú de usuario");

                // Validar Authorization header
                if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
                {
                    _logger.LogWarning("Header Authorization no encontrado");
                    return await CreateErrorResponseAsync(req, "Token de autorización requerido", HttpStatusCode.Unauthorized);
                }

                var authHeader = authHeaders.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Formato de Authorization header inválido");
                    return await CreateErrorResponseAsync(req, "Formato de token inválido", HttpStatusCode.Unauthorized);
                }

                var jwtToken = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogDebug("Token JWT extraído exitosamente");

                // Procesar la consulta
                var query = new GetUserMenuQuery(jwtToken);
                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    _logger.LogWarning("Error al procesar menú: {Message}", result.Message);
                    return await CreateErrorResponseAsync(req, result.Message ?? "Error al procesar menú", (HttpStatusCode)result.StatusCode);
                }

                // Escribir respuesta exitosa
                _logger.LogInformation("Menú de usuario obtenido exitosamente");
                return await CreateSuccessResponseAsync(req, result.Data ?? new MenuData(), result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al procesar solicitud de menú");
                return await CreateErrorResponseAsync(req, "Error interno del servidor", HttpStatusCode.InternalServerError);
            }
        }

     
        private static async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData req, string message, HttpStatusCode statusCode)
        {
            var errorResponse = BaseResponse<object>.Fail(message, (int)statusCode);
            var response = req.CreateResponse(statusCode);
            

            if (!response.Headers.Contains("Content-Type"))
            {
                response.Headers.Add("Content-Type", "application/json");
            }
            
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            await response.WriteStringAsync(json);
            return response;
        }


        private static async Task<HttpResponseData> CreateSuccessResponseAsync(HttpRequestData req, MenuData data, string? message = null)
        {
            var successResponse = BaseResponse<MenuData>.Success(data, message);
            var response = req.CreateResponse(HttpStatusCode.OK);
            
  
            if (!response.Headers.Contains("Content-Type"))
            {
                response.Headers.Add("Content-Type", "application/json");
            }
            
            var json = JsonSerializer.Serialize(successResponse, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            await response.WriteStringAsync(json);
            return response;
        }
    }
}
