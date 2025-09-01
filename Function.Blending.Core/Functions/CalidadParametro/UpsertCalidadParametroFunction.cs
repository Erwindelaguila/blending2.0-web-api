using Function.Blending.Core.Application.CalidadParametro.Commands;
using Function.Blending.Core.Application.CalidadParametro.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Function.Blending.Core.Functions.CalidadParametro;

public class UpsertCalidadParametroFunction
{
    private readonly ILogger<UpsertCalidadParametroFunction> _logger;
    private readonly IMediator _mediator;

    public UpsertCalidadParametroFunction(
        ILogger<UpsertCalidadParametroFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(FunctionNames.CalidadParametro.Upsert)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = ApiRoutes.Core.Configuraciones.CalidadParametroUpsert)] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("UpsertCalidadParametroFunction procesando...");

            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("El cuerpo de la solicitud está vacío.", "Error de validación", 400));
            }

            var batchDto = JsonSerializer.Deserialize<UpsertCalidadParametroBatchDTO>(body, HttpResponseHelper.GetJsonDeserializerOptions());

            if (batchDto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            // Convertir DTO a Command con RequestContext
            var cambios = batchDto.Cambios.Select(c => new CalidadParametroCambio
            {
                CalidadId = c.CalidadId,
                ParametroId = c.ParametroId,
                Valor = c.Valor
            }).ToList();

            var command = new UpsertCalidadParametroBatchCommand(cambios, req);

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Success(new { ProcessedCount = result }, 
                    $"Se procesaron {result} cambios exitosamente"));
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
        catch (ArgumentException ex)
        {
            var error = new { Field = ex.ParamName, Error = ex.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "Error de validación",
                400
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpsertCalidadParametroFunction: {Message}", ex.Message);

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
