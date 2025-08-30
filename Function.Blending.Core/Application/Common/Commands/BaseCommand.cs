using MediatR;

namespace Function.Blending.Core.Application.Common.Commands;

/// <summary>
/// Comando base que incluye el contexto de request para autenticación
/// Clean Architecture - mantiene la separación pero permite acceso al contexto
/// </summary>
/// <typeparam name="TResponse">Tipo de respuesta del comando</typeparam>
public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Contexto de request para extraer información de usuario
    /// Se mantiene como object para no depender de infraestructura específica
    /// </summary>
    public object RequestContext { get; }

    protected BaseCommand(object requestContext)
    {
        RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    }
}
