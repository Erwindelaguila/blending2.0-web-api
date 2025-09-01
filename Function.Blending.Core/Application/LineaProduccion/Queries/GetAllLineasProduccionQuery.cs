using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionQuery : BaseQuery<PagedResponse<LineaProduccionDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public LineaProduccionFilterDTO? Filters { get; }

    public GetAllLineasProduccionQuery(int page, int size, LineaProduccionFilterDTO? filters = null, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
