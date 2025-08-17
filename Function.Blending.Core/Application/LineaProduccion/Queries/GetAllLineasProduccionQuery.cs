using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionQuery : IRequest<PagedResponse<LineaProduccionDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public LineaProduccionFilterDTO? Filters { get; }

    public GetAllLineasProduccionQuery(int page, int size, LineaProduccionFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
