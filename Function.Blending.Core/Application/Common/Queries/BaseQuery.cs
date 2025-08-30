using MediatR;

namespace Function.Blending.Core.Application.Common.Queries;

/// <summary>
/// Clase base para todas las Queries que requieren autorización automática.
/// Permite al AuthorizationBehavior identificar qué requests necesitan autorización.
/// </summary>
public abstract class BaseQuery<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Contexto de request para extraer información de usuario
    /// Se mantiene como object para no depender de infraestructura específica
    /// </summary>
    public object RequestContext { get; }

    protected BaseQuery(object requestContext)
    {
        RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    }
}
