using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionQuery : IRequest<object>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
