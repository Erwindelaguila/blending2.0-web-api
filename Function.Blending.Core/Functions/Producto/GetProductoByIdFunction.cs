using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;

using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;

using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.Producto;


public class GetProductoByIdFunction
{
    private readonly IMediator _mediator;

    public GetProductoByIdFunction(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [RequireScopes("Administrador")]
    [Function(FunctionNames.Producto.GetById)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.Production.ProductoGetById)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var idString = query["id"];
            
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var productoId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de producto inválido o no proporcionado",
                    null,
                    400
                ));
            }

            var queryRequest = new GetProductoByIdQuery(productoId);
            var result = await _mediator.Send(queryRequest);

            if (result == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Producto no encontrado",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<ProductoDTO>.Success(result, "Producto obtenido correctamente"));
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
