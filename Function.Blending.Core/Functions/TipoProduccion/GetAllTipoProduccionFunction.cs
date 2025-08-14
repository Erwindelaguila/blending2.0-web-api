using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.TipoProduccion;

public class GetAllTipoProduccionFunction
{
    private readonly IMediator _mediator;

    public GetAllTipoProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.TipoProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionBase)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10; // Por defecto 10 registros por página

            var result = await _mediator.Send(new GetAllTipoProduccionQuery(page, size));
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result, "Tipos de producción obtenidos correctamente"));
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
