using System.Text.Json;
using System.Text.Json.Serialization;
using Function.Blending.Opt.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Callback;

public class ModelCallbackFunction
{
    private readonly ModelExecutionService _modelExecutionService;
    private readonly ILogger<ModelCallbackFunction> _logger;

    public ModelCallbackFunction(ModelExecutionService modelExecutionService, ILogger<ModelCallbackFunction> logger)
    {
        _modelExecutionService = modelExecutionService;
        _logger = logger;
    }

    [Function("ModelCallbackFunction")]
    public async Task Run(
        [ServiceBusTrigger("callback-queue", Connection = "ServiceBusConnection")] string message,
        FunctionContext context)
    {
        var log = context.GetLogger("ModelCallbackFunction");

        log.LogInformation("📥 Mensaje recibido desde Service Bus: {message}", message);

        try
        {
            var payload = JsonSerializer.Deserialize<ExecutionCallbackDto>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload is null || string.IsNullOrWhiteSpace(payload.ExecutionId))
            {
                log.LogWarning("⚠️ Mensaje inválido o ExecutionId no especificado.");
                return;
            }

            log.LogInformation("✅ Procesando ExecutionId: {ExecutionId}", payload.ExecutionId);

            await _modelExecutionService.MarkExecutionAsCompletedAsync(payload.ExecutionId, payload.Message, payload.Timestamp);

            log.LogInformation("✅ Ejecución actualizada correctamente en la base de datos.");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "❌ Error procesando el mensaje.");
            throw;
        }
    }
}

public class ExecutionCallbackDto
{
    [JsonPropertyName("executionId")]
    public string ExecutionId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}
