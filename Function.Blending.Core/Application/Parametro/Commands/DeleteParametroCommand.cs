using MediatR;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class DeleteParametroCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
