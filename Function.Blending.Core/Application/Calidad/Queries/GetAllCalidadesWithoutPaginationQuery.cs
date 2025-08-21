using Function.Blending.Core.Application.Calidad.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesWithoutPaginationQuery : IRequest<List<CalidadDTO>>
{
}
