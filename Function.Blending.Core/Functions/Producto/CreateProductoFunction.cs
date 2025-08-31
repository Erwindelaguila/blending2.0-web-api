using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Producto;

/// <summary>
/// Función para crear productos
/// Implementa patrón clean code con DTOs y validaciones declarativas
/// </summary>
public class CreateProductoFunction
{
    private readonly IMediator _mediator;

    public CreateProductoFunction(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function(FunctionNames.Producto.Create)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Post, Route = ApiRoutes.Core.Production.ProductoBase)] HttpRequestData req)
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
            var dto = JsonSerializer.Deserialize<CreateProductoRequestDTO>(body, HttpResponseHelper.GetJsonDeserializerOptions());
            if (dto == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req,
                    BaseResponse<object>.Fail("Error al deserializar el comando.", "Error de validación", 400));
            }
            
            // Crear comando desde DTO usando constructor
            var command = new CreateProductoCommand(
                dto.Codigo,
                dto.Nombre,
                dto.Descripcion,
                dto.CalidadId,
                dto.TipoProduccionId,
                dto.Activo,
                req
            );
            
            var result = await _mediator.Send(command);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<ProductoDTO>.Success(result, "Producto creado exitosamente"));
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
            var error = new { Field = "codigo", Error = "Ya existe un producto activo con este código" };
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
