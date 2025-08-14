using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
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
            // Obtener el registro actual para conservar valores
            var currentAgregado = await _agregadoRepository.GetByIdAsync(request.Id);
            if (currentAgregado == null)
                throw new InvalidOperationException("Agregado no encontrado");

            // Crear entidad con los nuevos datos, conservando valores actuales cuando no se proporcionan
            var agregadoToUpdate = new AgregadoEntity
            {
                Id = request.Id,
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo ?? currentAgregado.Activo, // Conservar valor actual si no se proporciona
                ModificadoPorId = request.ModificadoPorId,
                ModificadoEl = DateTime.UtcNow,
                // Preservar campos que no deben modificarse
                CreadoPorId = currentAgregado.CreadoPorId,
                CreadoEl = currentAgregado.CreadoEl,
                Eliminado = currentAgregado.Eliminado,
                EliminadoPorId = currentAgregado.EliminadoPorId,
                EliminadoEl = currentAgregado.EliminadoEl
            };

            await _agregadoRepository.UpdateAsync(agregadoToUpdate);

            return new AgregadoDTO
            {
                Id = agregadoToUpdate.Id,
                Codigo = agregadoToUpdate.Codigo,
                Nombre = agregadoToUpdate.Nombre,
                Descripcion = agregadoToUpdate.Descripcion,
                Activo = agregadoToUpdate.Activo,
                CreadoPorId = agregadoToUpdate.CreadoPorId,
                CreadoEl = agregadoToUpdate.CreadoEl,
                ModificadoPorId = agregadoToUpdate.ModificadoPorId,
                ModificadoEl = agregadoToUpdate.ModificadoEl
            };
        }
    }
}
