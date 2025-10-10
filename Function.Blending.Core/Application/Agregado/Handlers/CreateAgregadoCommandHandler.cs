using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Functions.Support.Execution;
using MediatR;
using System.Security.Claims;

namespace Function.Blending.Core.Application.Agregado.Handlers
{

    public class CreateAgregadoCommandHandler : IRequestHandler<CreateAgregadoCommand, AgregadoDTO>
    {
        private readonly IAgregadoRepository _agregadoRepository;
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public CreateAgregadoCommandHandler(IAgregadoRepository agregadoRepository, IFunctionContextAccessor functionContextAccessor)
        {
            _agregadoRepository = agregadoRepository ?? throw new ArgumentNullException(nameof(agregadoRepository));
            _functionContextAccessor = functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));
        }

        public async Task<AgregadoDTO> Handle(CreateAgregadoCommand request, CancellationToken cancellationToken)
        {
            // Obtener el usuario actual del JWT
            var currentUserId = GetCurrentUserId();
            
            var agregado = new AgregadoEntity
            {
                Id = Guid.NewGuid(),
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo ?? true,
                CreadoPorId = currentUserId, 
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
