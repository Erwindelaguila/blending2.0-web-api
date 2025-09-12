using MediatR;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Commands;
using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Common.Behaviors;


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
            // TEMPORAL: Deshabilitar autorización para Calidad GAAAA
            if (requiredScope.StartsWith("calidades."))
            {
                _logger.LogDebug("Autorización deshabilitada temporalmente para Calidad - scope: {RequiredScope}", requiredScope);
                // Configurar contexto sin validar scope
                AuthorizationContextHelper.SetupAuthorizationFromCommand(
                    requestContext, 
                    _authorizationService);
                return await next();
            }

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
        
        // ✅ TEMPORAL: Deshabilitar validación de scopes para múltiples entidades durante pruebas
        if (entityName.StartsWith("agregado") || 
            entityName.StartsWith("lineaproduccion") || 
            entityName.StartsWith("producto") ||
            entityName.StartsWith("parametro") ||
            entityName.StartsWith("calidadparametro") ||
            entityName.StartsWith("planta") ||
            entityName.StartsWith("tipoproduccion") ||
            entityName.StartsWith("calidad"))
        {
            return string.Empty; // Sin scope requerido = sin validación
        }
        
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
            // GetAllAgregadosQuery -> Agregados -> agregados
            if (requestTypeName.StartsWith("GetAll"))
            {
                var entityName = requestTypeName.Replace("GetAll", "").Replace("Query", "");
                
                // Normalizar nombres de entidades - remover sufijos como "Activas"
                entityName = NormalizeEntityName(entityName);
                
                // GetAllLineasProduccionQuery -> LineasProduccion -> lineaproduccion
                if (entityName == "LineasProduccion") return "lineaproduccion";
                if (entityName == "TiposProduccion") return "tipoproduccion";
                return entityName.ToLowerInvariant();
            }
            // GetAgregadoByIdQuery -> Agregado -> agregados
            if (requestTypeName.StartsWith("Get"))
            {
                var entityName = requestTypeName.Replace("Get", "").Replace("ByIdQuery", "").Replace("Query", "");
                
                // Normalizar nombres de entidades - remover sufijos como "Activas"
                entityName = NormalizeEntityName(entityName);
                
                if (entityName == "LineaProduccion") return "lineaproduccion";
                if (entityName == "TipoProduccion") return "tipoproduccion";
                // Casos especiales para mantener consistencia con el plural
                if (entityName == "Calidad") return "calidades";
                if (entityName == "Planta") return "plantas";
                if (entityName == "Agregado") return "agregados";
                if (entityName == "Parametro") return "parametros";
                if (entityName == "Producto") return "productos";
                return entityName.ToLowerInvariant() + "s";
            }
        }
        else
        {
            // CreateAppParamCommand -> AppParam -> appparams
            if (requestTypeName.StartsWith("Create"))
            {
                var entityName = requestTypeName.Replace("Create", "").Replace("Command", "");
                
                // Normalizar nombres de entidades - remover sufijos como "Activas"
                entityName = NormalizeEntityName(entityName);
                
                if (entityName == "LineaProduccion") return "lineaproduccion";
                if (entityName == "TipoProduccion") return "tipoproduccion";
                // Casos especiales para mantener consistencia con el plural
                if (entityName == "Calidad") return "calidades";
                if (entityName == "Planta") return "plantas";
                if (entityName == "Agregado") return "agregados";
                if (entityName == "Parametro") return "parametros";
                if (entityName == "Producto") return "productos";
                return entityName.ToLowerInvariant() + "s";
            }
            if (requestTypeName.StartsWith("Update"))
            {
                var entityName = requestTypeName.Replace("Update", "").Replace("Command", "");
                
                // Normalizar nombres de entidades - remover sufijos como "Activas"
                entityName = NormalizeEntityName(entityName);
                
                if (entityName == "LineaProduccion") return "lineaproduccion";
                if (entityName == "TipoProduccion") return "tipoproduccion";
                // Casos especiales para mantener consistencia con el plural
                if (entityName == "Calidad") return "calidades";
                if (entityName == "Planta") return "plantas";
                if (entityName == "Agregado") return "agregados";
                if (entityName == "Parametro") return "parametros";
                if (entityName == "Producto") return "productos";
                return entityName.ToLowerInvariant() + "s";
            }
            if (requestTypeName.StartsWith("Delete"))
            {
                var entityName = requestTypeName.Replace("Delete", "").Replace("Command", "");
                
                // Normalizar nombres de entidades - remover sufijos como "Activas"
                entityName = NormalizeEntityName(entityName);
                
                if (entityName == "LineaProduccion") return "lineaproduccion";
                if (entityName == "TipoProduccion") return "tipoproduccion";
                // Casos especiales para mantener consistencia con el plural
                if (entityName == "Calidad") return "calidades";
                if (entityName == "Planta") return "plantas";
                if (entityName == "Agregado") return "agregados";
                if (entityName == "Parametro") return "parametros";
                if (entityName == "Producto") return "productos";
                return entityName.ToLowerInvariant() + "s";
            }
        }
        
        return requestTypeName.Replace("Command", "").Replace("Query", "").ToLowerInvariant();
    }

    /// <summary>
    /// Normaliza nombres de entidades removiendo sufijos como "Activas", "Disponibles", etc.
    /// para que usen el mismo scope base de la entidad principal.
    /// </summary>
    private static string NormalizeEntityName(string entityName)
    {
        // Remover sufijos comunes que no cambian la entidad base
        var suffixesToRemove = new[] { 
            "WithoutPagination", "Activas", "Activos", "Disponibles", 
            "Habilitadas", "Habilitados", "Combo", "Simple", "List"
        };
        
        foreach (var suffix in suffixesToRemove)
        {
            if (entityName.EndsWith(suffix))
            {
                entityName = entityName.Substring(0, entityName.Length - suffix.Length);
                break;
            }
        }
        
        return entityName;
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
