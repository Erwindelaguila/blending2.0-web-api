using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetPlantaByIdQuery : BaseQuery<PlantaDTO?>
{
    public Guid Id { get; }

    public GetPlantaByIdQuery(Guid id)
    {
        Id = id;
    }
}
