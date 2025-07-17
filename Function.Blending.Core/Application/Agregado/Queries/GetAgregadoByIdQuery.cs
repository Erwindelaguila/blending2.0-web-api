using Function.Blending.Core.Application.Agregado.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Queries;

public class GetAgregadoByIdQuery : IRequest<AgregadoDTO?>
{
    public Guid Id { get; set; }
}
