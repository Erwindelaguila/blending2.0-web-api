using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class CreateAppParamFunction
{
    private readonly IMediator _mediator;

    public CreateAppParamFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Request body is empty",
                    "Cuerpo de la solicitud vacío",
                    400
                ));
            }

            var jsonDocument = JsonDocument.Parse(body);
            var root = jsonDocument.RootElement;

            // Validar campos requeridos
            if (!root.TryGetProperty("key", out var keyElement) || string.IsNullOrWhiteSpace(keyElement.GetString()))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Key is required",
                    "La clave es requerida",
                    400
                ));
            }

            if (!root.TryGetProperty("value", out var valueElement) || string.IsNullOrWhiteSpace(valueElement.GetString()))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Value is required",
                    "El valor es requerido",
                    400
                ));
            }

            if (!root.TryGetProperty("creadoPorId", out var creadoPorIdElement) || 
                !Guid.TryParse(creadoPorIdElement.GetString(), out var creadoPorId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Valid CreadoPorId is required",
                    "Se requiere un CreadoPorId válido",
                    400
                ));
            }

            var command = new CreateAppParamCommand(
                key: keyElement.GetString()!,
                value: valueElement.GetString()!,
                description: root.TryGetProperty("description", out var descElement) ? descElement.GetString() : null,
                category: null, // El frontend no envía esto - valor por defecto
                group: null, // El frontend no envía esto - valor por defecto
                isActive: root.TryGetProperty("isActive", out var activeElement) ? activeElement.GetBoolean() : true,
                isInternal: false, // Valor por defecto - el frontend no envía esto
                isVisible: true, // Valor por defecto - el frontend no envía esto
                isDisableable: true, // Valor por defecto - el frontend no envía esto
                isRemovable: true, // Valor por defecto - el frontend no envía esto
                creadoPorId: creadoPorId
            );

            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(
                result, 
                "AppParam creado exitosamente"
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
