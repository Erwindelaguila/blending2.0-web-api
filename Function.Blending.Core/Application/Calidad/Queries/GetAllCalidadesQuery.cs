using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesQuery : IRequest<PagedResponse<CalidadDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public CalidadFilterDTO? Filters { get; }

    public GetAllCalidadesQuery(int page, int size, CalidadFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}