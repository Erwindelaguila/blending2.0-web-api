using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;

using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;


namespace Function.Blending.Core.Functions.Agregado;

public class CreateAgregadoFunction
{
    private readonly IMediator _mediator;

    public CreateAgregadoFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireScopes("Administrador")]
    [Function(FunctionNames.Agregado.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Post, Route = ApiRoutes.Core.Production.AgregadoBase)] HttpRequestData req)
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
            
            var dto = JsonSerializer.Deserialize<CreateAgregadoRequestDTO>(body, HttpResponseHelper.GetJsonDeserializerOptions());

            if (dto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            var command = new CreateAgregadoCommand(
                dto.Codigo,
                dto.Nombre,
                dto.Descripcion,
                dto.Activo
            );

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AgregadoDTO>.Success(result, "Agregado creado exitosamente"));
        }
        catch (ValidationException ex)
        {
            // Crear errores específicos por campo con mensajes claros
            var validationErrors = ex.Errors.Select(error => new 
            { 
                Campo = error.PropertyName, 
                Error = error.ErrorMessage
            }).ToList();

            var errorSummary = ex.Errors.Count() == 1 
                ? ex.Errors.First().ErrorMessage
                : $"Se encontraron {ex.Errors.Count()} errores de validación.";

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                validationErrors,
                errorSummary,
                400
            ));
        }
        catch (ArgumentException ex) when (ex.ParamName == "codigo")
        {
            var error = new { Field = "codigo", Error = "Ya existe un agregado activo con este código" };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "Código duplicado",
                409
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
