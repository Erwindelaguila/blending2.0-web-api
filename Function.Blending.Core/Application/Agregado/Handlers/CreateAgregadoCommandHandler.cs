using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers
{
    public class CreateAgregadoCommandHandler : IRequestHandler<CreateAgregadoCommand, AgregadoDTO>
    {
        private readonly IAgregadoRepository _agregadoRepository;
        public CreateAgregadoCommandHandler(IAgregadoRepository agregadoRepository)
        {
            _agregadoRepository = agregadoRepository;
        }

        public async Task<AgregadoDTO> Handle(CreateAgregadoCommand request, CancellationToken cancellationToken)
        {
            var agregado = new AgregadoEntity
            {
                Id = Guid.NewGuid(),
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo ?? true,
                CreadoPorId = request.CreadoPorId,
                CreadoEl = DateTime.UtcNow
            };
            await _agregadoRepository.CreateAsync(agregado);
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
