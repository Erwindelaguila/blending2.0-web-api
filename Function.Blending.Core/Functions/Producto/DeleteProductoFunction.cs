using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Producto;


public class DeleteProductoFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; // ✅ NUEVO: Inyección para JWT

    public DeleteProductoFunction(IMediator mediator, IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _headerExtractor = headerExtractor; // ✅ NUEVO: Asignar extractor JWT
    }

    [Function(FunctionNames.Producto.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Production.ProductoBase + "/{id}")] HttpRequestData req,
        string id)
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

            if (!Guid.TryParse(id, out var productoId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            var command = new DeleteProductoCommand(productoId, req);

            var result = await _mediator.Send(command);

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Producto no encontrado",
                    "Recurso no encontrado",
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<bool>.Success(result, "Producto eliminado exitosamente"));
        }
        catch (EntityInUseException ex)
        {
            var error = new { Message = ex.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "No se puede eliminar",
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
            // ✅ NUEVO: Limpiar contexto JWT al final de la función
            AuthorizationService.ClearCurrentContext();
        }
    }
}
