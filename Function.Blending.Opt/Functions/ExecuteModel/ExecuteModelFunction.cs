using System.Net;
using System.Text.Json;
using Function.Blending.Opt.Application.Services;
using Function.Blending.Opt.Models.Requests;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.ExecuteModel;

public class ExecuteModelFunction
{
    private readonly ILogger _logger;
    private readonly ModelExecutionService _executionService;

    public ExecuteModelFunction(ILoggerFactory loggerFactory, ModelExecutionService executionService)
    {
        _logger = loggerFactory.CreateLogger<ExecuteModelFunction>();
        _executionService = executionService;
    }

    [Function("ExecuteModelFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "model/execute")] HttpRequestData req)
    {
        var body = await req.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<ExecuteModelRequest>(body);

        if (string.IsNullOrWhiteSpace(data.Planta) || string.IsNullOrWhiteSpace(data.TipoModelo))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Datos incompletos.");
            return bad;
        }

        var executionId = await _executionService.RegistrarEjecucionAsync(data.Planta, data.TipoModelo);

        // (Opcional) Aquí se llamaría a la API externa para iniciar la ejecución
        // await _externalApiClient.EnviarModeloAsync(data, executionId);

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(new { executionId });

        return response;
    }
}