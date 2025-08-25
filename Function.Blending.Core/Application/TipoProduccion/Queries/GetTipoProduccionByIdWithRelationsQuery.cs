using Function.Blending.Core.Application.TipoProduccion.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetTipoProduccionByIdWithRelationsQuery : IRequest<TipoProduccionDTO?>
{
    public Guid Id { get; set; }

    public GetTipoProduccionByIdWithRelationsQuery(Guid id)
    {
        Id = id;
    }
}
