using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class UpdateLineaProduccionFunction
{
    private readonly IMediator _mediator;

    public UpdateLineaProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.LineaProduccion.Update)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Put, Route = ApiRoutes.Core.Production.LineaProduccionGetById)] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }

            var jsonDocument = JsonDocument.Parse(body);
            var root = jsonDocument.RootElement;

            var query = HttpUtility.ParseQueryString(req.Url.Query);
            if (!Guid.TryParse(query["id"], out var lineaProduccionId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            if (!root.TryGetProperty("codigo", out var codigoElement) || string.IsNullOrWhiteSpace(codigoElement.GetString()))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Código es requerido",
                    "El código es requerido",
                    400
                ));
            }

            if (!root.TryGetProperty("nombre", out var nombreElement) || string.IsNullOrWhiteSpace(nombreElement.GetString()))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Nombre es requerido", 
                    "El nombre es requerido",
                    400
                ));
            }

            var command = new UpdateLineaProduccionCommand(
                id: lineaProduccionId,
                codigo: codigoElement.GetString()!,
                nombre: nombreElement.GetString()!,
                descripcion: root.TryGetProperty("descripcion", out var descElement) ? descElement.GetString() : null,
                activo: root.TryGetProperty("activo", out var activoElement) ? activoElement.GetBoolean() : null,
                requestContext: req
            );

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<LineaProduccionDTO>.Success(result, "Línea de Producción actualizada exitosamente"));
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
        catch (EntityInUseException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                new { Error = ex.Message, Code = ex.ErrorCode },
                "Conflicto de regla de negocio",
                409
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
        finally
        {
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
