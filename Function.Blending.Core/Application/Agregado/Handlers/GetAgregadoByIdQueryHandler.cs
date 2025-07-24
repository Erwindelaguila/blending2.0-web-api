using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using MediatR;


namespace Function.Blending.Core.Application.Agregado.Handlers
{
    public class GetAgregadoByIdQueryHandler : IRequestHandler<GetAgregadoByIdQuery, AgregadoDTO?>
    {
        private readonly IAgregadoRepository _agregadoRepository;

        public GetAgregadoByIdQueryHandler(IAgregadoRepository agregadoRepository)
        {
            _agregadoRepository = agregadoRepository;
        }

        public async Task<AgregadoDTO?> Handle(GetAgregadoByIdQuery request, CancellationToken cancellationToken)
        {
            var agregado = await _agregadoRepository.GetByIdAsync(request.Id);
            
            if (agregado == null)
                return null;

            return new AgregadoDTO
            {
                Id = agregado.Id,
                Codigo = agregado.Codigo,
                Nombre = agregado.Nombre,
                Descripcion = agregado.Descripcion,
                Activo = agregado.Activo,
                CreadoPorId = agregado.CreadoPorId,
                CreadoEl = agregado.CreadoEl,
                ModificadoPorId = agregado.ModificadoPorId,
                ModificadoEl = agregado.ModificadoEl
            };
        }
    }
}
