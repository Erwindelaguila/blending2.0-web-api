using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Wrappers;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesQuery : BaseQuery<PagedResponse<CalidadDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public CalidadFilterDTO? Filters { get; }

    public GetAllCalidadesQuery(int page, int size, CalidadFilterDTO? filters = null, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}