using MediatR;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Common.Behaviors;


public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAuditService _auditService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

    public AuditBehavior(
        IAuditService auditService,
        IAuthorizationService authorizationService,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _auditService = auditService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Ejecutar el handler principal
        var response = await next();

        // Auditoría automática solo para Commands que heredan de BaseCommand
        if (request is BaseCommand<TResponse> command)
        {
            try
            {
                await LogCommandExecution(command, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en auditoría para command {CommandType}", typeof(TRequest).Name);
            }
        }

        return response;
    }

    private async Task LogCommandExecution(BaseCommand<TResponse> command, TResponse response)
    {
        var commandType = command.GetType().Name;
        var entityName = ExtractEntityName(commandType);
        var action = ExtractAction(commandType);

        switch (action.ToLower())
        {
            case "create":
                if (response != null)
                    await _auditService.LogCreateAsync(entityName, GetEntityId(response), response);
                break;
            
            case "update":
                if (response != null)
                    await _auditService.LogUpdateAsync(entityName, GetEntityId(command), new {}, response);
                break;
            
            case "delete":
                await _auditService.LogDeleteAsync(entityName, GetEntityId(command), command);
                break;
        }
    }

    private static string ExtractEntityName(string commandType)
    {
        // CreateAppParamCommand -> AppParam
        // UpdateAgregadoCommand -> Agregado
        if (commandType.StartsWith("Create"))
            return commandType.Replace("Create", "").Replace("Command", "");
        if (commandType.StartsWith("Update"))
            return commandType.Replace("Update", "").Replace("Command", "");
        if (commandType.StartsWith("Delete"))
            return commandType.Replace("Delete", "").Replace("Command", "");
        
        return commandType.Replace("Command", "");
    }

    private static string ExtractAction(string commandType)
    {
        // CreateAppParamCommand -> Create
        if (commandType.StartsWith("Create")) return "Create";
        if (commandType.StartsWith("Update")) return "Update";
        if (commandType.StartsWith("Delete")) return "Delete";
        
        return "Unknown";
    }

    private static string GetEntityId(object obj)
    {
        // Intentar obtener ID o Key de la respuesta/command
        var type = obj.GetType();
        
        var idProperty = type.GetProperty("Id") ?? type.GetProperty("Key") ?? type.GetProperty("Codigo");
        if (idProperty != null)
        {
            var value = idProperty.GetValue(obj);
            return value?.ToString() ?? "Unknown";
        }

        return "Unknown";
    }
}
