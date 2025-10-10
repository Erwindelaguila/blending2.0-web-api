using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Agregado.Queries;


public class GetAllAgregadosQuery : BaseQuery<PagedResponse<AgregadoDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public AgregadoFilterDTO? Filters { get; }

    public GetAllAgregadosQuery(
        int page, 
        int size, 
        AgregadoFilterDTO? filters)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}

