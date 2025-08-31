using MediatR;

namespace Function.Blending.Core.Application.Common.Queries;

public abstract class BaseQuery<TResponse> : IRequest<TResponse>
{

    public object RequestContext { get; }

    protected BaseQuery(object requestContext)
    {
        RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    }
}
