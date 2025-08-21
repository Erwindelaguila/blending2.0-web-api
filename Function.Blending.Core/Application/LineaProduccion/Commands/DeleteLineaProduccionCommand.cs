using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class DeleteLineaProduccionCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteLineaProduccionCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
