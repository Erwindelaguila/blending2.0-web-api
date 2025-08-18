using MediatR;

namespace Function.Blending.Core.Application.Planta.Queries;

public class GetPagedPlantasQuery : IRequest<object>
{
    public int Page { get; }
    public int Size { get; }

    public GetPagedPlantasQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }
}
