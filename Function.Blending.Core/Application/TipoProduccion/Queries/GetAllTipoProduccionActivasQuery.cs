using Function.Blending.Core.Application.TipoProduccion.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Queries;

public class GetAllTipoProduccionActivasQuery : IRequest<List<TipoProduccionActivaDTO>>
{
}
