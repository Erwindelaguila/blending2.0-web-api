using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Planta;

public class CreatePlantaFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; 

    public CreatePlantaFunction(
        IMediator mediator,
        IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator;
        _headerExtractor = headerExtractor;
    }

    [Function(FunctionNames.Planta.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.Planta.Base)] HttpRequestData req)
    {
        try
        {
            // ✅ NUEVO: Establecer contexto JWT al inicio de la función
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

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
            
            var dto = JsonSerializer.Deserialize<CreatePlantaRequestDTO>(body, HttpResponseHelper.GetJsonDeserializerOptions());

            if (dto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }

            var command = new CreatePlantaCommand(
                dto.Codigo,
                dto.Nombre,
                dto.Descripcion,
                dto.NumeroRuma,
                dto.Activo,
                req
            );

            var result = await _mediator.Send(command);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PlantaDTO>.Success(result, "Planta creada exitosamente"));
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
            var error = new { Field = "codigo", Error = "Ya existe una planta activa con este código" };
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
           
            AuthorizationService.ClearCurrentContext();
        }
    }
}
