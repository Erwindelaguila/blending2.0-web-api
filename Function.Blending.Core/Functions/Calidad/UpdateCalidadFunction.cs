using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Calidad;

public class UpdateCalidadFunction
{
    private readonly IMediator _mediator;

    public UpdateCalidadFunction(IMediator mediator)
    {
        _mediator = mediator;
    }
    
     [Function(FunctionNames.Calidad.Update)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function,HttpMethods.Put, Route = ApiRoutes.core.Production.Calidad)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            var (isValid, errorField) = JsonValidationHelper.ValidateBooleanProperties(body, "activo", "noConforme");

            if (!isValid)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail($"El campo '{errorField}' debe ser booleano (true o false o null).", "Error de validación", 400));
            }
            
            var command = JsonSerializer.Deserialize<UpdateCalidadCommand>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        
            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<CalidadDTO>.Success(result,"Create Calidad"));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errors,
                "Validación fallida. Por favor, revise los campos.",
                401
            ));
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
                //StackTrace = ex.StackTrace
            };
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errorMessage,
                null,
                500
            ));
        }
        
    }
    
    
}