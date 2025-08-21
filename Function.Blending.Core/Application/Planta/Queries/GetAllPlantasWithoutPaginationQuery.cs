using Function.Blending.Core.Application.Planta.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetAllPlantasWithoutPaginationQuery : IRequest<List<PlantaDTO>>
{
}
