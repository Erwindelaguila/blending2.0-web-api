using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Function.Blending.Core.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de auditoría que registra todas las acciones
/// usando el User ID obtenido de los headers X-User-*.
/// 
/// En producción este servicio podría enviar logs a:
/// - Application Insights
/// - Azure Table Storage
/// - SQL Server
/// - Service Bus para procesamiento asíncrono
/// </summary>
public class AuditService : IAuditService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IAuthorizationService authorizationService, ILogger<AuditService> logger)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registra una acción de creación con auditoría completa.
    /// </summary>
    public async Task LogCreateAsync(string entityName, string entityId, object data)
    {
        var auditLog = CreateAuditLog("CREATE", entityName, entityId, data);
        await WriteAuditLogAsync(auditLog);
    }

    /// <summary>
    /// Registra una acción de actualización con datos antes y después.
    /// </summary>
    public async Task LogUpdateAsync(string entityName, string entityId, object oldData, object newData)
    {
        var auditLog = CreateAuditLog("UPDATE", entityName, entityId, new { Before = oldData, After = newData });
        await WriteAuditLogAsync(auditLog);
    }

    /// <summary>
    /// Registra una acción de eliminación lógica.
    /// </summary>
    public async Task LogDeleteAsync(string entityName, string entityId, object data)
    {
        var auditLog = CreateAuditLog("DELETE", entityName, entityId, data);
        await WriteAuditLogAsync(auditLog);
    }

    /// <summary>
    /// Registra una acción de consulta con filtros aplicados.
    /// </summary>
    public async Task LogReadAsync(string entityName, object? filters = null)
    {
        var auditLog = CreateAuditLog("READ", entityName, null, filters);
        await WriteAuditLogAsync(auditLog);
    }

    /// <summary>
    /// Crea un objeto de auditoría con toda la información necesaria.
    /// </summary>
    private object CreateAuditLog(string action, string entityName, string? entityId, object? data)
    {
        return new
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            UserId = _authorizationService.GetCurrentUserId(),
            UserName = _authorizationService.GetCurrentUserName(),
            UserScopes = _authorizationService.GetCurrentUserScopes(),
            Data = data
        };
    }

    /// <summary>
    /// Escribe el log de auditoría. En desarrollo usa ILogger,
    /// en producción se podría enviar a Application Insights, Storage, etc.
    /// </summary>
    private async Task WriteAuditLogAsync(object auditLog)
    {
        try
        {
            var jsonLog = JsonSerializer.Serialize(auditLog, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _logger.LogInformation("=== AUDIT LOG ===");
            _logger.LogInformation("{AuditLog}", jsonLog);
            
            // TODO: En producción, enviar a Application Insights o Storage
            // await _applicationInsights.TrackEventAsync("UserAction", auditLog);
            // await _storageService.SaveAuditLogAsync(auditLog);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escribiendo audit log");
        }
    }
}
