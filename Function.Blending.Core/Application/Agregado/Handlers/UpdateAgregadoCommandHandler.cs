using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers
{
    /// <summary>
    /// Handler para actualizar agregados
    /// Reutiliza servicios de autenticación de Function.Blending.Auth
    /// </summary>
    public class UpdateAgregadoCommandHandler : IRequestHandler<UpdateAgregadoCommand, AgregadoDTO>
    {
        private readonly IAgregadoRepository _agregadoRepository;
        private readonly IAuthorizationService _authorizationService;

        public UpdateAgregadoCommandHandler(
            IAgregadoRepository agregadoRepository,
            IAuthorizationService authorizationService)
        {
            _agregadoRepository = agregadoRepository ?? throw new ArgumentNullException(nameof(agregadoRepository));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        public async Task<AgregadoDTO> Handle(UpdateAgregadoCommand request, CancellationToken cancellationToken)
        {
            // Obtener usuario actual usando servicios reutilizados de Auth
            // Obtener user ID desde headers (via AuthorizationService)
            var currentUserIdString = _authorizationService.GetCurrentUserId();
            if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            {
                throw new UnauthorizedAccessException("User ID inválido en headers");
            }
            
            // Obtener el registro actual para conservar valores
            var currentAgregado = await _agregadoRepository.GetByIdAsync(request.Id);
            if (currentAgregado == null)
                throw new InvalidOperationException("Agregado no encontrado");

            // Validar regla de negocio: no se puede inactivar si está siendo usado por TipoProducción activo
            if (currentAgregado.Activo && request.Activo == false)
            {
                var isUsedByActiveTipoProduccion = await _agregadoRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
                if (isUsedByActiveTipoProduccion)
                {
                    throw new EntityInUseException("el Agregado", "está siendo usado por al menos un Tipo de Producción activo");
                }
            }

            // Crear entidad con los nuevos datos, conservando valores actuales cuando no se proporcionan
            var agregadoToUpdate = new AgregadoEntity
            {
                Id = request.Id,
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo ?? currentAgregado.Activo, // Conservar valor actual si no se proporciona
                ModificadoPorId = currentUserId,
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
