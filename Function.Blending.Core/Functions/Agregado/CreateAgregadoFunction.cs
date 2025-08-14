using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Agregado;

public class CreateAgregadoFunction
{
    private readonly IMediator _mediator;

    public CreateAgregadoFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Agregado.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.Production.AgregadoBase)] HttpRequestData req)
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

            var command = JsonSerializer.Deserialize<CreateAgregadoCommand>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            if (command == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AgregadoDTO>.Success(result, "Agregado creado exitosamente"));
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
