using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Functions.Support.Execution;
using MediatR;
using System.Security.Claims;

namespace Function.Blending.Core.Application.Agregado.Handlers
{
    public class UpdateAgregadoCommandHandler : IRequestHandler<UpdateAgregadoCommand, AgregadoDTO>
    {
        private readonly IAgregadoRepository _agregadoRepository;
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public UpdateAgregadoCommandHandler(IAgregadoRepository agregadoRepository, IFunctionContextAccessor functionContextAccessor)
        {
            _agregadoRepository = agregadoRepository ?? throw new ArgumentNullException(nameof(agregadoRepository));
            _functionContextAccessor = functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));
        }

        public async Task<AgregadoDTO> Handle(UpdateAgregadoCommand request, CancellationToken cancellationToken)
        {
            var currentAgregado = await _agregadoRepository.GetByIdAsync(request.Id);
            if (currentAgregado == null)
                throw new InvalidOperationException("Agregado no encontrado");

            if (currentAgregado.Activo && request.Activo == false)
            {
                var isUsedByActiveTipoProduccion = await _agregadoRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
                if (isUsedByActiveTipoProduccion)
                {
                    throw new EntityInUseException("el Agregado", "está siendo usado por al menos un Tipo de Producción activo");
                }
            }

            var agregadoToUpdate = new AgregadoEntity
            {
                Id = request.Id,
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo ?? currentAgregado.Activo, 
                ModificadoPorId = GetCurrentUserId(), // Usuario real del JWT
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

        private Guid GetCurrentUserId()
        {
            try
            {
                var context = _functionContextAccessor.Current;
                if (context?.Items.TryGetValue(MiscellaneousConstants.Principal, out var principalObj) == true &&
                    principalObj is ClaimsPrincipal principal)
                {
                    var userIdString = principal.GetUserId();
                    if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                    {
                        return userId;
                    }
                }
            }
            catch
            {
                // Si hay error obteniendo el usuario, usar fallback
            }
            
            // Fallback: usuario del sistema
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }
}
