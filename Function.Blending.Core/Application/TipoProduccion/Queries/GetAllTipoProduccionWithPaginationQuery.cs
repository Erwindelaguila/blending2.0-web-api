using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetAllTipoProduccionWithPaginationQuery : IRequest<PagedResponse<TipoProduccionDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public TipoProduccionFilterDTO? Filters { get; }

    public GetAllTipoProduccionWithPaginationQuery(int page, int size, TipoProduccionFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
