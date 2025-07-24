using System.Text.Json;
using FluentValidation;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Producto;

public class GetAllProductosFunction
{
    private readonly IMediator _mediator;

    public GetAllProductosFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Producto.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.ProductoBase)] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllProductosQuery());
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<ProductoDTO>>.Success(result));
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
