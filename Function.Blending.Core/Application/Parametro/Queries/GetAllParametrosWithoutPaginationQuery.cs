using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetAllParametrosWithoutPaginationQuery : IRequest<List<ParametroDTO>>
{
}
