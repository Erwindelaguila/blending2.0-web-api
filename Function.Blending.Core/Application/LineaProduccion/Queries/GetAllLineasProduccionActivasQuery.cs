using Function.Blending.Core.Application.LineaProduccion.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionActivasQuery : IRequest<List<LineaProduccionActivaDTO>>
{
}
