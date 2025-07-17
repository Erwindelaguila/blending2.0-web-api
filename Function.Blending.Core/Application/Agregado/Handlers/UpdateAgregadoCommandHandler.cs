using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;


namespace Function.Blending.Core.Application.Agregado.Handlers
{
    public class UpdateAgregadoCommandHandler : IRequestHandler<UpdateAgregadoCommand, AgregadoDTO>
    {
        private readonly IAgregadoRepository _agregadoRepository;
        public UpdateAgregadoCommandHandler(IAgregadoRepository agregadoRepository)
        {
            _agregadoRepository = agregadoRepository;
        }
        public async Task<AgregadoDTO> Handle(UpdateAgregadoCommand request, CancellationToken cancellationToken)
        {
            var agregado = await _agregadoRepository.GetByIdAsync(request.Id);

            if (agregado == null)
                throw new ArgumentException($"Agregado con ID {request.Id} no encontrado");

            agregado.Codigo = request.Codigo ?? agregado.Codigo;
            agregado.Nombre = request.Nombre ?? agregado.Nombre;
            agregado.Descripcion = request.Descripcion ?? agregado.Descripcion;
            agregado.Activo = request.Activo ?? agregado.Activo;
            agregado.ModificadoPorId = request.ModificadoPorId;
            agregado.ModificadoEl = DateTime.Now;

            await _agregadoRepository.UpdateAsync(agregado);

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
