using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetAllParametrosWithPaginationQuery : IRequest<PagedResponse<ParametroDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public ParametroFilterDTO? Filters { get; }

    public GetAllParametrosWithPaginationQuery(int page, int size, ParametroFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
