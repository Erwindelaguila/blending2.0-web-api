using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetAllParametrosQuery : IRequest<object>
{
    public int Page { get; }
    public int Size { get; }

    public GetAllParametrosQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }
}
