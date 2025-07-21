using MediatR;
using Function.Blending.Core.Application.LineaProduccion.DTOs;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetLineaProduccionByIdQuery : IRequest<LineaProduccionDTO?>
{
    public Guid Id { get; }

    public GetLineaProduccionByIdQuery(Guid id)
    {
        Id = id;
    }
}
