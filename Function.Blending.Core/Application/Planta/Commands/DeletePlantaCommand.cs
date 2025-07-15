using MediatR;

namespace Function.Blending.Core.Application.Planta.Commands;

public class DeletePlantaCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
