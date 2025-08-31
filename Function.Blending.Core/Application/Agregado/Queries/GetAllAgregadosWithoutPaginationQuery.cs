using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Agregado.Queries;

/// <summary>
/// Query para obtener todos los agregados sin paginación
/// Hereda de BaseQuery para autorización automática
/// </summary>
public class GetAllAgregadosWithoutPaginationQuery : BaseQuery<List<AgregadoDTO>>
{
    public GetAllAgregadosWithoutPaginationQuery(object requestContext) : base(requestContext)
    {
    }
}
