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

public class CreateAppParamFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuditService _auditService;

    public CreateAppParamFunction(
        IMediator mediator, 
        IAuthorizationService authorizationService,
        IAuditService auditService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
    }

    [Function(FunctionNames.AppParam.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
            // PASO 0: CONFIGURAR HEADERS PARA AUTHORIZATION SERVICE
            SetupAuthorizationHeaders(req);

            // PASO 1: VALIDAR AUTORIZACIÓN
            if (!_authorizationService.HasRequiredScope("appparams.write"))
            {
                var unauthorizedResponse = req.CreateResponse(System.Net.HttpStatusCode.Forbidden);
                await unauthorizedResponse.WriteAsJsonAsync(BaseResponse<string>.Fail("Acceso denegado: Se requiere permiso 'appparams.write'"));
                return unauthorizedResponse;
            }

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

            var command = new CreateAppParamCommand(
                key: keyElement.GetString()!,
                value: valueElement.GetString()!,
                description: root.TryGetProperty("description", out var descElement) ? descElement.GetString() : null,
                category: null, // El frontend no envía esto - valor por defecto
                group: null, // El frontend no envía esto - valor por defecto
                isActive: true, // USUARIO: Siempre true
                isInternal: false, // USUARIO: Siempre false = puede modificar código  
                isVisible: true, // USUARIO: Siempre true
                isDisableable: false, // USUARIO: Siempre false = se puede inactivar
                isRemovable: true, // USUARIO: Siempre true = se puede eliminar
                requestContext: req // Clean Architecture: contexto para autenticación
            );

            var result = await _mediator.Send(command);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(
                result, 
                "AppParam creado exitosamente"
            ));
        }
        catch (DuplicateKeyException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                "Código duplicado",
                400
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
            // Limpiar headers al final del request
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }

    /// <summary>
    /// Configura los headers de autorización para Azure Functions.
    /// Extrae los headers del HttpRequestData y los configura en el AuthorizationService.
    /// </summary>
    private static void SetupAuthorizationHeaders(HttpRequestData req)
    {
        var headers = new Dictionary<string, string>();
        
        foreach (var header in req.Headers)
        {
            var values = header.Value?.ToArray();
            if (values != null && values.Length > 0)
            {
                headers[header.Key] = values[0];
            }
        }

        AuthorizationService.SetCurrentRequestHeaders(headers);
    }
}
