using Function.Blending.Core.Application.CalidadParametro.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Function.Blending.Core.Functions.CalidadParametro;

public class GetCalidadParametroMatrizFunction
{
    private readonly ILogger<GetCalidadParametroMatrizFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; 

    public GetCalidadParametroMatrizFunction(
        ILogger<GetCalidadParametroMatrizFunction> logger,
        IMediator mediator,
        IAuthorizationHeaderExtractor headerExtractor)
    {
        _logger = logger;
        _mediator = mediator;
        _headerExtractor = headerExtractor;
    }

    [Function(FunctionNames.CalidadParametro.GetMatriz)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Configuraciones.CalidadParametroMatriz)] HttpRequestData req)
    {
        try
        {
          
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            _logger.LogInformation("GetCalidadParametroMatrizFunction procesando...");

            var query = new GetCalidadParametroMatrizQuery();
            var result = await _mediator.Send(query);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Success(result, "Matriz de calidad-parámetros obtenida exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetCalidadParametroMatrizFunction: {Message}", ex.Message);

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
