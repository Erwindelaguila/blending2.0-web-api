using Function.Blending.Core.Application.Planta.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetPlantaByIdQuery : IRequest<PlantaDTO?>
{
    public Guid Id { get; set; }
}
