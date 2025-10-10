using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Security;
using System.Security.Claims;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class CreateLineaProduccionCommandHandler : IRequestHandler<CreateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public CreateLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<LineaProduccionDTO> Handle(CreateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var linea = new LineaProduccionEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            CreadoPorId = currentUserId,
            CreadoEl = DateTime.UtcNow
        };

        await _lineaProduccionRepository.CreateAsync(linea);

        return new LineaProduccionDTO
        {
            Id = linea.Id,
            Codigo = linea.Codigo,
            Nombre = linea.Nombre,
            Descripcion = linea.Descripcion,
            Activo = linea.Activo,
            CreadoEl = linea.CreadoEl
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
