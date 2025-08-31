using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionActivasQuery : BaseQuery<List<LineaProduccionActivaDTO>>
{
    public GetAllLineasProduccionActivasQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
