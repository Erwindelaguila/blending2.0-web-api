using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.Common.Wrappers;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesQuery : BaseQuery<CalidadesResponseDTO>
{
    public int Page { get; }
    public int Size { get; }
    public CalidadFilterDTO? Filters { get; }
    public bool IsGlobal { get; }
    public GetAllCalidadesQuery(int page, int size, CalidadFilterDTO? filters = null, bool isGlobal = false)
    {
        Page = page;
        Size = size;
        Filters = filters;
        IsGlobal = isGlobal;
    }
}