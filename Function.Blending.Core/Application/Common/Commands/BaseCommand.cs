using MediatR;

namespace Function.Blending.Core.Application.Common.Commands;


public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
    public object RequestContext { get; }

    protected BaseCommand(object requestContext)
    {
        RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    }
}
