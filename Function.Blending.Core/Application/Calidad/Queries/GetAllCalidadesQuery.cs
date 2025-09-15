using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Wrappers;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesQuery : BaseQuery<CalidadesResponseDTO>
{
    public int Page { get; }
    public int Size { get; }
    public CalidadFilterDTO? Filters { get; }
    public bool IsGlobal { get; }
    public GetAllCalidadesQuery(int page, int size, CalidadFilterDTO? filters = null, HttpRequestData? requestContext = null, bool isGlobal = false) : base(requestContext!)
    {
        Page = page;
        Size = size;
        Filters = filters;
        IsGlobal = isGlobal;
    }
}