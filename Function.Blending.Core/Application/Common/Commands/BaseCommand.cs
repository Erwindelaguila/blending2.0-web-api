using MediatR;

namespace Function.Blending.Core.Application.Common.Commands;


public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
        public BaseCommand() { }
}
