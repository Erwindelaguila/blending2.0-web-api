using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetAllPlantasWithPaginationQuery : IRequest<PlantaResponseDTO>
{
    public int Page { get; }
    public int Size { get; }
    public PlantaFilterDTO? Filters { get; }
    public bool IsHarina { get; }

    public GetAllPlantasWithPaginationQuery(int page, int size, PlantaFilterDTO? filters = null ,  bool isHarina = false)
    {
        Page = page;
        Size = size;
        Filters = filters;
        IsHarina = isHarina;
    }
}
