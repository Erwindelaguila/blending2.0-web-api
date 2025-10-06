using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.LineaProduccion.DTOs;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionActivasQuery : BaseQuery<List<LineaProduccionActivaDTO>>
{
    public GetAllLineasProduccionActivasQuery()
    {
    }
}
