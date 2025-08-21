using Function.Blending.Core.Application.TipoProduccion.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetAllTipoProduccionWithoutPaginationQuery : IRequest<List<TipoProduccionDTO>>
{
    public TipoProduccionFilterDTO? Filters { get; }

    public GetAllTipoProduccionWithoutPaginationQuery(TipoProduccionFilterDTO? filters = null)
    {
        Filters = filters;
    }
}
