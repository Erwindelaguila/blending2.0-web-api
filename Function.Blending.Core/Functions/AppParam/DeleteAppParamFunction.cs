using System.Net;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.AppParam;

public class DeleteAppParamFunction
{
    private readonly ILogger<DeleteAppParamFunction> _logger;
    private readonly IMediator _mediator;

    public DeleteAppParamFunction(ILogger<DeleteAppParamFunction> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.AppParam.Base + "/{key}")] HttpRequestData req,
        string key)
    {
        _logger.LogInformation("DeleteAppParam function processed a request.");

        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Key is required", 
                    "El key es requerido",
                    400
                ));
            }

            var command = new DeleteAppParamCommand(key);
            
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
    }
}
