using MediatR;

namespace Function.Blending.Core.Application.Planta.Commands;

public class DeletePlantaCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeletePlantaCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
