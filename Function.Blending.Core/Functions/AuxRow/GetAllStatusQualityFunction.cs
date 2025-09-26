using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.AuxRow.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.AuxRow;

public class GetAllStatusQualityFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllStatusQualityFunction> _logger;
    private readonly IAuthorizationHeaderExtractor _headerExtractor;

    public GetAllStatusQualityFunction(IMediator mediator, ILogger<GetAllStatusQualityFunction> logger,
        IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator;
        _logger = logger;
        _headerExtractor = headerExtractor;
    }

    [Function(nameof(GetAllStatusQualityFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.AuxRow.StatusQuality)]
        HttpRequestData req)
    {
        try
        {
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            var queryRequest = new GetAllStatusQualityQuery();

            var result = await _mediator.Send(queryRequest);
            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<List<StatusQualityDTO>>.Success(result ?? new List<StatusQualityDTO>(),
                    "Plantas obtenidas correctamente"));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (parámetros inválidos) - devolver 400
            _logger.LogWarning("Validation error in GetAllPlantas: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            // Error interno del servidor - devolver 500
            _logger.LogError(ex, "Unexpected error in GetAllPlantas");
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