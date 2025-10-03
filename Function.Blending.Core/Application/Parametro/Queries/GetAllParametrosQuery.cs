using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetAllParametrosQuery : BaseQuery<PagedResponse<ParametroDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public ParametroFilterDTO? Filters { get; }

    public GetAllParametrosQuery(int page, int size, ParametroFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
