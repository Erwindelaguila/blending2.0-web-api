using MediatR;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Commands;
using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Common.Behaviors;

/// <summary>
/// Behavior que maneja autorización automática para todos los Commands.
/// Implementa Clean Architecture separando la responsabilidad de autorización.
/// </summary>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

    public AuthorizationBehavior(
        IAuthorizationService authorizationService,
        ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    {
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Autorización automática para Commands y Queries que heredan de clases base
        object? requestContext = null;
        string? requiredScope = null;

        if (request is BaseCommand<TResponse> command)
        {
            requestContext = command.RequestContext;
            requiredScope = GetRequiredScope(command.GetType().Name, isQuery: false);
        }
        else if (request is BaseQuery<TResponse> query)
        {
            requestContext = query.RequestContext;
            requiredScope = GetRequiredScope(query.GetType().Name, isQuery: true);
        }

        if (requestContext != null && !string.IsNullOrEmpty(requiredScope))
        {
            // Configurar contexto de autorización PARA TODA LA CADENA
            AuthorizationContextHelper.SetupAuthorizationFromCommand(
                requestContext, 
                _authorizationService);

            _logger.LogDebug("Validando scope requerido: {RequiredScope} para request: {RequestType}", 
                requiredScope, typeof(TRequest).Name);

            if (!_authorizationService.HasRequiredScope(requiredScope))
            {
                _logger.LogWarning("Acceso denegado. Usuario no tiene scope: {RequiredScope} para request: {RequestType}", 
                    requiredScope, typeof(TRequest).Name);
                
                throw new UnauthorizedAccessException($"No tiene permisos para ejecutar esta operación. Scope requerido: {requiredScope}");
            }

            _logger.LogDebug("Autorización exitosa para scope: {RequiredScope}", requiredScope);
        }
        else if (requestContext != null)
        {
            // Aunque no requiera scope específico, configurar contexto para CommandHandlers
            AuthorizationContextHelper.SetupAuthorizationFromCommand(
                requestContext, 
                _authorizationService);
        }

        // Ejecutar el siguiente handler en el pipeline (contexto se mantiene)
        return await next();
    }

    private static string GetRequiredScope(string requestTypeName, bool isQuery)
    {
        var entityName = ExtractEntityName(requestTypeName, isQuery).ToLower();
        
        if (isQuery)
        {
            // Todas las queries requieren scope de lectura
            return $"{entityName}.read";
        }
        else
        {
            // Commands requieren scope según la acción
            var action = ExtractAction(requestTypeName).ToLower();
            return action switch
            {
                "create" => $"{entityName}.write",
                "update" => $"{entityName}.write", 
                "delete" => $"{entityName}.write",
                _ => string.Empty // Para otros comandos sin scope específico
            };
        }
    }

    private static string ExtractEntityName(string requestTypeName, bool isQuery)
    {
        if (isQuery)
        {
            // GetAllAgregadosQuery -> agregados (pluralizado para queries)
            if (requestTypeName.StartsWith("GetAll"))
                return requestTypeName.Replace("GetAll", "").Replace("Query", "");
            if (requestTypeName.StartsWith("Get"))
                return requestTypeName.Replace("Get", "").Replace("ByIdQuery", "").Replace("Query", "") + "s";
        }
        else
        {
            // CreateAppParamCommand -> appparams
            if (requestTypeName.StartsWith("Create"))
                return requestTypeName.Replace("Create", "").Replace("Command", "") + "s";
            if (requestTypeName.StartsWith("Update"))
                return requestTypeName.Replace("Update", "").Replace("Command", "") + "s";
            if (requestTypeName.StartsWith("Delete"))
                return requestTypeName.Replace("Delete", "").Replace("Command", "") + "s";
        }
        
        return requestTypeName.Replace("Command", "").Replace("Query", "");
    }

    private static string ExtractAction(string commandType)
    {
        // CreateAppParamCommand -> Create
        if (commandType.StartsWith("Create")) return "Create";
        if (commandType.StartsWith("Update")) return "Update";
        if (commandType.StartsWith("Delete")) return "Delete";
        
        return "Unknown";
    }
}
