using MediatR;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetAllTipoProduccionQuery : IRequest<object>
{
    public int Page { get; }
    public int Size { get; }

    public GetAllTipoProduccionQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }
}
