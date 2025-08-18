using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.Calidad;

public class GetAllCalidadFunction
{
    private readonly IMediator _mediator;

    public GetAllCalidadFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Calidad.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.CalidadBase)]HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);

            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var result =  await _mediator.Send(new GetAllCalidadesQuery(page, size));
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result,"Calidades obtenidas correctamente"));
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