using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetAllPlantasQuery : BaseQuery<PlantaResponseDTO>
{
    public int Page { get; }
    public int Size { get; }
    public PlantaFilterDTO? Filters { get; }
    public bool IsHarina { get; }

    public GetAllPlantasQuery(int page, int size, PlantaFilterDTO? filters = null, bool isHarina = false, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Page = page;
        Size = size;
        Filters = filters;
        IsHarina = isHarina;
    }
}
