using MediatR;
using Function.Blending.Core.Application.LineaProduccion.DTOs;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetAllLineasProduccionQuery : IRequest<List<LineaProduccionDTO>>;
