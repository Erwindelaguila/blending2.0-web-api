using System.Net;
using Function.Blending.Upload.Exceptions;
using Function.Blending.Upload.Models;
using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class GetCadmioProcess
{
    private readonly CadmioService _cadmioService;
    private readonly ILogger<GetCadmioProcess> _logger;

    public GetCadmioProcess(CadmioService cadmioService , ILogger<GetCadmioProcess> logger)
    {
        _cadmioService = cadmioService;
        _logger = logger;
    }
    
    public async Task<List<CadmioResult>> ExecuteAsync(HttpRequestData req)
    {
        
        var requestDto = await req.ReadFromJsonAsync<ObtenerCadmioRequestDto>();

        if (requestDto == null || requestDto.Rumas.Length == 0)
        {
            _logger.LogError("Debe enviar al menos una ruma.");
            throw new BadRequestException("Debe enviar al menos una ruma.");
        }
        return await _cadmioService.ObtenerCadmioAsync(requestDto);
    }
}