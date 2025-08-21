using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class DeleteTipoProduccionCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteTipoProduccionCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
