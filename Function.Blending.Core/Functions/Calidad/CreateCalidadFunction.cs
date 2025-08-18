using System.Text.Json;
using Function.Blending.Core.Application.Calidad.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using FluentValidation;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;

namespace Function.Blending.Core.Functions.Calidad;

public class CreateCalidadFunction
{
    private readonly IMediator _mediator;

    public CreateCalidadFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Calidad.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.Production.CalidadBase)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }
            
            var (isValid, errorField) = JsonValidationHelper.ValidateBooleanProperties(body, "activo", "noConforme");

            if (!isValid)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail($"El campo '{errorField}' debe ser booleano (true o false o null).", "Error de validación", 400));
            }
            
            var command = JsonSerializer.Deserialize<CreateCalidadCommand>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            if (command == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }
        
            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result,"Calidad creada exitosamente"));
        }
        catch (ArgumentException ex) when (ex.ParamName == "codigo")
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                "El código de la calidad ya existe. Por favor, use un código diferente.",
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