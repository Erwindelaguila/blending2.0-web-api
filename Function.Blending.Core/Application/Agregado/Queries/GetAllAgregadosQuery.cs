using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Queries;

public class GetAllAgregadosQuery : IRequest<PagedResponse<AgregadoDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public AgregadoFilterDTO? Filters { get; }

    public GetAllAgregadosQuery(int page, int size, AgregadoFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}

