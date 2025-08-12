using Function.Blending.Core.Application.Agregado.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Queries;

public class GetAllAgregadosQuery : IRequest<object>
{
    public int Page { get; }
    public int Size { get; }

    public GetAllAgregadosQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }
}

