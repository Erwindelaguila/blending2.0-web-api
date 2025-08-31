using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

/// <summary>
/// Query para obtener todos los tipos de producción sin paginación
/// Hereda de BaseQuery para autorización automática
/// </summary>
public class GetAllTipoProduccionWithoutPaginationQuery : BaseQuery<List<TipoProduccionDTO>>
{
    public TipoProduccionFilterDTO? Filters { get; }

    public GetAllTipoProduccionWithoutPaginationQuery(
        TipoProduccionFilterDTO? filters,
        object requestContext) : base(requestContext)
    {
        Filters = filters;
    }
}
