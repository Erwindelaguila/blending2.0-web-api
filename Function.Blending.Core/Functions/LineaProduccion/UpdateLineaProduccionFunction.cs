using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class UpdateLineaProduccionFunction
{
    private readonly IMediator _mediator;

    public UpdateLineaProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("UpdateLineaProduccion")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Put, Route = ApiRoutes.Core.Production.LineaProduccionBase)] HttpRequestData req)
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
            var command = JsonSerializer.Deserialize<UpdateLineaProduccionCommand>(body, HttpResponseHelper.GetJsonDeserializerOptions());
            if (command == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<LineaProduccionDTO>.Success(result, "Linea de Producción actualizada exitosamente"));
        }
        catch (ValidationException ex)
        {
            var validationErrors = ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                validationErrors,
                "Errores de validación",
                400
            ));
        }
        catch (ArgumentException ex) when (ex.ParamName == "codigo")
        {
            var error = new { Field = "codigo", Error = "Ya existe una línea de producción activa con este código" };
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
