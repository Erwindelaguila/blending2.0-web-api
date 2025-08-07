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
            // Crear la respuesta inmediatamente para evitar ObjectDisposedException
            var response = req.CreateResponse();
            
            try
            {
                _logger.LogInformation("Iniciando solicitud de menú de usuario");

                // Validar Authorization header
                if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
                {
                    _logger.LogWarning("Header Authorization no encontrado");
                    await WriteErrorResponseAsync(response, "Token de autorización requerido", HttpStatusCode.Unauthorized);
                    return response;
                }

                var authHeader = authHeaders.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Formato de Authorization header inválido");
                    await WriteErrorResponseAsync(response, "Formato de token inválido", HttpStatusCode.Unauthorized);
                    return response;
                }

                var jwtToken = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogDebug("Token JWT extraído exitosamente");

                // Procesar la consulta
                var query = new GetUserMenuQuery(jwtToken);
                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    _logger.LogWarning("Error al procesar menú: {Message}", result.Message);
                    await WriteErrorResponseAsync(response, result.Message ?? "Error al procesar menú", (HttpStatusCode)result.StatusCode);
                    return response;
                }

                // Escribir respuesta exitosa
                _logger.LogInformation("Menú de usuario obtenido exitosamente");
                await WriteSuccessResponseAsync(response, result.Data ?? new MenuData(), result.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al procesar solicitud de menú");
                await WriteErrorResponseAsync(response, "Error interno del servidor", HttpStatusCode.InternalServerError);
                return response;
            }
        }

        /// <summary>
        /// Escribe una respuesta de error de forma segura
        /// </summary>
        private static async Task WriteErrorResponseAsync(HttpResponseData response, string message, HttpStatusCode statusCode)
        {
            var errorResponse = BaseResponse<object>.Fail(message, (int)statusCode);
            
            response.StatusCode = statusCode;
            response.Headers.Add("Content-Type", "application/json");
            
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            await response.WriteStringAsync(json);
        }

        /// <summary>
        /// Escribe una respuesta exitosa de forma segura
        /// </summary>
        private static async Task WriteSuccessResponseAsync(HttpResponseData response, MenuData data, string? message = null)
        {
            var successResponse = BaseResponse<MenuData>.Success(data, message);
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            
            var json = JsonSerializer.Serialize(successResponse, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            await response.WriteStringAsync(json);
        }
    }
}
