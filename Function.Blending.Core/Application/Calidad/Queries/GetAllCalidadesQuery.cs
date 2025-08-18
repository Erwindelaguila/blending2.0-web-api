using Function.Blending.Core.Application.Calidad.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesQuery : IRequest<object>
{
    public int Page { get; }
    public int Size { get; }

    public GetAllCalidadesQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }
}