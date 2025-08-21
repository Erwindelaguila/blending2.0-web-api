using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Commands;

public class DeletePlantaCommand : IRequest<BaseResponse<object>>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeletePlantaCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
