using System.Net;
using Function.Blending.Core.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Producto;

public class GetAllProductsFunction
{
    private readonly IMediator _mediator;

    public GetAllProductsFunction(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Function("GetAllProducts")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req,
        FunctionContext executionContext)
    {
        try
        {
            var productosDto = await _mediator.Send(new GetAllProductsQuery());
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(productosDto);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}