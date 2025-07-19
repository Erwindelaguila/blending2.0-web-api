using MediatR;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetTipoProduccionByIdQuery : IRequest<TipoProduccionDTO?>
{
    public Guid Id { get; set; }
}
