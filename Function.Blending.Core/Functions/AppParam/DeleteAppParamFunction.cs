using System.Net;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.AppParam;

public class DeleteAppParamFunction
{
    private readonly ILogger<DeleteAppParamFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuditService _auditService;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; 

    public DeleteAppParamFunction(ILogger<DeleteAppParamFunction> logger, IMediator mediator, IAuthorizationService authorizationService, IAuditService auditService, IAuthorizationHeaderExtractor headerExtractor)
    {
        _logger = logger;
        _mediator = mediator;
        _authorizationService = authorizationService;
        _auditService = auditService;
        _headerExtractor = headerExtractor;
    }


    private void SetupAuthorizationHeaders(HttpRequestData req)
    {
        // TEMPORALMENTE DESHABILITADO - Solo probando con Agregado
        /*
        var headers = new Dictionary<string, string>();
        
        foreach (var header in req.Headers)
        {
            headers[header.Key] = header.Value.FirstOrDefault() ?? "";
        }
        
        AuthorizationService.SetCurrentRequestHeaders(headers);
        */
    }

    [Function(FunctionNames.AppParam.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Delete, Route = ApiRoutes.Core.AppParam.Base + "/{key}")] HttpRequestData req,
        string key)
    {
        _logger.LogInformation("DeleteAppParam function processed a request.");

        try
        {
   
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            // ===== AUTORIZACIÓN =====
            // Configurar headers desde HttpRequestData para Azure Functions
            SetupAuthorizationHeaders(req);
            
            // Validar autorización - scope requerido para eliminar app params
            if (!_authorizationService.HasRequiredScope("appparams.write"))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Access denied: insufficient permissions",
                    "Acceso denegado: permisos insuficientes",
                    403
                ));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Key is required", 
                    "El key es requerido",
                    400
                ));
            }

            var command = new DeleteAppParamCommand(key, req);
            
            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(
                result, 
                $"AppParam '{key}' eliminado exitosamente"
            ));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("AppParam not found: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message, 
                "Parámetro no encontrado",
                404
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message, 
                "Operación no permitida",
                400
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting AppParam");
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                "An error occurred while deleting the AppParam", 
                "Error interno del servidor",
                500
            ));
        }
        finally
        {
       
            AuthorizationService.ClearCurrentContext();
        }
    }
}
