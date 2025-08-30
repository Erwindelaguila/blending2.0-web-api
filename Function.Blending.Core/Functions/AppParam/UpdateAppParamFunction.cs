using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class UpdateAppParamFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuditService _auditService;

    public UpdateAppParamFunction(IMediator mediator, IAuthorizationService authorizationService, IAuditService auditService)
    {
        _mediator = mediator;
        _authorizationService = authorizationService;
        _auditService = auditService;
    }

    /// <summary>
    /// Configura los headers de autorización desde HttpRequestData para Azure Functions.
    /// Esto es necesario porque Azure Functions Worker no usa HttpContext de la misma manera que ASP.NET Core.
    /// </summary>
    private void SetupAuthorizationHeaders(HttpRequestData req)
    {
        var headers = new Dictionary<string, string>();
        
        foreach (var header in req.Headers)
        {
            headers[header.Key] = header.Value.FirstOrDefault() ?? "";
        }
        
        // Configurar headers en el AuthorizationService estático para Azure Functions
        AuthorizationService.SetCurrentRequestHeaders(headers);
    }

    [Function(FunctionNames.AppParam.Update)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Put, Route = ApiRoutes.Core.AppParam.GetById + "/{key}")] HttpRequestData req,
        string key)
    {
        try
        {
            // ===== AUTORIZACIÓN =====
            // Configurar headers desde HttpRequestData para Azure Functions
            SetupAuthorizationHeaders(req);
            
            // Validar autorización - scope requerido para actualizar app params
            if (!_authorizationService.HasRequiredScope("appparams.write"))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Access denied: insufficient permissions",
                    "Acceso denegado: permisos insuficientes",
                    403
                ));
            }
            // ===== FIN AUTORIZACIÓN =====

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

            // Validar que el key de la ruta no esté vacío
            if (string.IsNullOrWhiteSpace(key))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Key is required in route",
                    "El key es requerido en la ruta",
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

            var command = new UpdateAppParamCommand(
                key: key, // Key actual de la ruta
                newKey: root.TryGetProperty("key", out var newKeyElement) ? newKeyElement.GetString() : null, // Nuevo key del body
                value: valueElement.GetString()!,
                description: root.TryGetProperty("description", out var descElement) ? descElement.GetString() : null,
                category: null, // El frontend no envía esto - mantener valor existente
                group: null, // El frontend no envía esto - mantener valor existente
                isActive: root.TryGetProperty("isActive", out var activeElement) ? activeElement.GetBoolean() : null,
                isInternal: null, // El frontend no envía esto - mantener valor existente
                isVisible: null, // El frontend no envía esto - mantener valor existente
                isDisableable: null, // El frontend no envía esto - mantener valor existente
                isRemovable: null, // El frontend no envía esto - mantener valor existente
                requestContext: req // Clean Architecture: contexto para autenticación
            );

            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(
                result, 
                "AppParam actualizado exitosamente"
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errors,
                "Validación fallida. Por favor, revise los campos.",
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
        finally
        {
            // Limpiar headers del contexto de Azure Functions
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
