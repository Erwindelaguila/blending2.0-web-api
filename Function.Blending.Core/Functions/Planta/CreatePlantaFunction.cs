using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Planta;

public class CreatePlantaFunction
{
    private readonly IMediator _mediator;

    public CreatePlantaFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Planta.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.Planta.Base)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }
            
            var (isValid, errorField) = JsonValidationHelper.ValidateBooleanProperties(body, "activo");

            if (!isValid)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail($"El campo '{errorField}' debe ser booleano (true o false o null).", "Error de validación", 400));
            }
            
            var command = JsonSerializer.Deserialize<CreatePlantaCommand>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            if (command == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }
        
            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result,"Planta creada exitosamente"));
        }
        catch (ArgumentException ex) when (ex.ParamName == "codigo")
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                "El código de la planta ya existe. Por favor, use un código diferente.",
                "Código duplicado",
                409
            ));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errors,
                "Validación fallida. Por favor, revise los campos.",
                400
            ));
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
            };
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errorMessage,
                null,
                500
            ));
        }
    }
}
