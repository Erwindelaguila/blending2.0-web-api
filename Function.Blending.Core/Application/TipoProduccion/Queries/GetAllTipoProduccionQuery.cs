using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

/// <summary>
/// Query para obtener todos los tipos de producción con paginación
/// Hereda de BaseQuery para autorización automática
/// </summary>
public class GetAllTipoProduccionQuery : BaseQuery<PagedResponse<TipoProduccionDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public TipoProduccionFilterDTO? Filters { get; }

    public GetAllTipoProduccionQuery(
        int page, 
        int size, 
        TipoProduccionFilterDTO? filters,
        object requestContext) : base(requestContext)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
