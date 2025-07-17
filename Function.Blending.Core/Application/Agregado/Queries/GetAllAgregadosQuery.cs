using Function.Blending.Core.Application.Agregado.DTOs;
using MediatR;


namespace Function.Blending.Core.Application.Agregado.Queries
{
    public class GetAllAgregadosQuery : IRequest<List<AgregadoDTO>>
    {
    }
}
