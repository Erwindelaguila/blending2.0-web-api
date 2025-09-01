using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;


public class GetTipoProduccionByIdQuery : BaseQuery<TipoProduccionDTO?>
{
    public Guid Id { get; }

    public GetTipoProduccionByIdQuery(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
