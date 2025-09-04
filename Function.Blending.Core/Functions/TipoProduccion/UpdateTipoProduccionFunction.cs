using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.TipoProduccion;

/// <summary>
/// Función para actualizar tipos de producción
/// Implementa patrón clean code con DTOs y validaciones declarativas
/// </summary>
public class UpdateTipoProduccionFunction
{
    private readonly IMediator _mediator;

    public UpdateTipoProduccionFunction(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function(FunctionNames.TipoProduccion.Update)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Put, Route = ApiRoutes.Core.Production.TipoProduccionGetById)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }

            // Validar propiedades booleanas usando helper común
            var (isValid, errorField) = JsonValidationHelper.ValidateBooleanProperties(body, "activo");
            if (!isValid)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail($"El campo '{errorField}' debe ser booleano (true o false o null).", "Error de validación", 400));
            }

            // Deserializar usando DTO con validaciones declarativas
            var dto = JsonSerializer.Deserialize<UpdateTipoProduccionRequestDTO>(body, HttpResponseHelper.GetJsonDeserializerOptions());
            if (dto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            // Crear comando desde DTO usando constructor
            var command = new UpdateTipoProduccionCommand(
                dto.Id,
                dto.Codigo,
                dto.Nombre,
                dto.Descripcion,
                dto.LineaProduccionId,
                dto.AgregadoId,
                dto.Activo,
                req
            );
            
            var result = await _mediator.Send(command);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<TipoProduccionDTO>.Success(result, "Tipo de Producción actualizado exitosamente"));
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
            var error = new { Field = "codigo", Error = "Ya existe un tipo de producción activo con este código" };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "Código duplicado",
                409
            ));
        }
        catch (BusinessRuleException ex)
        {
            var error = new { Message = ex.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "No se puede activar por elementos inactivos",
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
        finally
        {
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
