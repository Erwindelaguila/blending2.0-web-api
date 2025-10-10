using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class UpdateCalidadCommandHandler : IRequestHandler<UpdateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpdateCalidadCommandHandler(
        ICalidadRepository calidadRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _calidadRepository = calidadRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<CalidadDTO> Handle(UpdateCalidadCommand request, CancellationToken cancellationToken)
    {
        var currentCalidad = await _calidadRepository.GetByIdAsync(request.Id);
        if (currentCalidad == null)
            throw new BusinessRuleException($"Calidad with ID {request.Id} not found.", 
                "CALIDAD_NOT_FOUND");

        if (currentCalidad.Activo && request.Activo == false)
        {
            var isUsedByActiveProducto = await _calidadRepository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
            }
        }

        var modificadoPorId = GetCurrentUserId();

        var calidad = new CalidadEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            CodigoMaterial = request.CodigoMaterial,
            Descripcion = request.Descripcion,
            NoConforme = request.NoConforme ?? false,
            Activo = request.Activo ?? true,
            ModificadoPorId = modificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var calidadActualizada = await _calidadRepository.UpdateAndReturnAsync(calidad);

        return new CalidadDTO
        {
            Id = calidadActualizada.Id,
            Codigo = calidadActualizada.Codigo,
            Nombre = calidadActualizada.Nombre,
            CodigoMaterial = calidadActualizada.CodigoMaterial,
            Descripcion = calidadActualizada.Descripcion,
            NoConforme = calidadActualizada.NoConforme,
            Activo = calidadActualizada.Activo,
            CreadoPorId = calidadActualizada.CreadoPorId,
            CreadoEl = calidadActualizada.CreadoEl,
            ModificadoPorId = calidadActualizada.ModificadoPorId,
            ModificadoEl = calidadActualizada.ModificadoEl,
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