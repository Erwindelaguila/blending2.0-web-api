using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionWithoutPaginationQuery : BaseQuery<List<LineaProduccionDTO>>
{
    public GetAllLineasProduccionWithoutPaginationQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
