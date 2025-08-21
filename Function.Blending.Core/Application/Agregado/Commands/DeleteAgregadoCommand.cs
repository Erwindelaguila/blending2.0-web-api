using MediatR;


namespace Function.Blending.Core.Application.Agregado.Commands;

public class DeleteAgregadoCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteAgregadoCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
