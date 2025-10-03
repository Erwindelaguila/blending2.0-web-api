using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class CreateCalidadCommandHandler : IRequestHandler<CreateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public CreateCalidadCommandHandler(
        ICalidadRepository calidadRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _calidadRepository = calidadRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<CalidadDTO> Handle(CreateCalidadCommand request, CancellationToken cancellationToken)
    {
        var creadoPorId = GetCurrentUserId();

        var calidad = new CalidadEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            CodigoMaterial = request.CodigoMaterial,
            Descripcion = request.Descripcion,
            NoConforme = request.NoConforme ?? false,
            Activo = request.Activo ?? true,
            CreadoPorId = creadoPorId,
            CreadoEl = DateTime.UtcNow,
            Eliminado = false
        };

        await _calidadRepository.CreateAsync(calidad);

        return new CalidadDTO
        {
            Id = calidad.Id,
            Codigo = calidad.Codigo,
            Nombre = calidad.Nombre,
            CodigoMaterial = calidad.CodigoMaterial,
            Descripcion = calidad.Descripcion,
            NoConforme = calidad.NoConforme,
            Activo = calidad.Activo,
            CreadoPorId = calidad.CreadoPorId,
            CreadoEl = calidad.CreadoEl,
            ModificadoPorId = calidad.ModificadoPorId,
            ModificadoEl = calidad.ModificadoEl
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