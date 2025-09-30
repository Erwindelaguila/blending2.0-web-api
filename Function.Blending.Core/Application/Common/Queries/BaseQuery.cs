using MediatR;
using System;

namespace Function.Blending.Core.Application.Common.Queries
{
    public abstract class BaseQuery<TResponse> : IRequest<TResponse>
    {
        public object RequestContext { get; }

        // Constructor vacío (para compatibilidad)
        protected BaseQuery() { }

        // Constructor con validación (tu mejora)
        protected BaseQuery(object requestContext)
        {
            RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }
    }
}
