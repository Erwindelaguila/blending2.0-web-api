using MediatR;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class DeleteParametroCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteParametroCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
