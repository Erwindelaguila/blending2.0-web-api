using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;

using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;

using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class UpdateAppParamFunction
{
    private readonly IMediator _mediator;

    public UpdateAppParamFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireScopes("Administrador,Calidad")]
    [Function(FunctionNames.AppParam.Update)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Put, Route = ApiRoutes.Core.AppParam.GetById + "/{key}")] HttpRequestData req,
        string key)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }

            var (isValid, errorField) = JsonValidationHelper.ValidateBooleanProperties(body, "isActive", "isInternal", "isVisible", "isDisableable", "isRemovable");

            if (!isValid)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail($"El campo '{errorField}' debe ser booleano (true o false).", "Error de validación", 400));
            }

            if (string.IsNullOrEmpty(key))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "La clave del parámetro es requerida.",
                    "Error de validación",
                    400
                ));
            }

            var dto = JsonSerializer.Deserialize<UpdateAppParamRequestDTO>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el cuerpo de la solicitud.", "Error de validación", 400));
            }

            var command = new UpdateAppParamCommand(
                key: key, // Key actual de la ruta
                newKey: dto.Key, // Nuevo key del body (puede ser null)
                value: dto.Value,
                description: dto.Description,
                category: dto.Category,
                group: dto.Group,
                isActive: dto.IsActive,
                isInternal: dto.IsInternal,
                isVisible: dto.IsVisible,
                isDisableable: dto.IsDisableable,
                isRemovable: dto.IsRemovable
            );

            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(
                result, 
                "Parámetro actualizado correctamente"
            ));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { Campo = e.PropertyName, Error = e.ErrorMessage }).ToList();
            
            var errorSummary = errors.Count == 1 
                ? errors.First().Error
                : $"Se encontraron {errors.Count} errores de validación";
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errors,
                errorSummary,
                400
            ));
        }
        catch (EntityInUseException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                new { Error = ex.Message, Code = ex.ErrorCode },
                "Conflicto de regla de negocio",
                409
            ));
        }
        catch (DuplicateKeyException ex)
        {
            var error = new { Field = "key", Error = ex.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "Código duplicado",
                409
            ));
        }
        catch (BusinessRuleException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                "Error de validación de negocio",
                400
            ));
        }
        catch (KeyNotFoundException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                "AppParam no encontrado",
                404
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
