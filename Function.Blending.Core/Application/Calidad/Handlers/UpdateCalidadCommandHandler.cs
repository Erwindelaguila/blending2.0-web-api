using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class UpdateCalidadCommandHandler : IRequestHandler<UpdateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpdateCalidadCommandHandler(ICalidadRepository calidadRepository, IAuthorizationService authorizationService)
    {
        _calidadRepository = calidadRepository;
        _authorizationService = authorizationService;
    }

    public async Task<CalidadDTO> Handle(UpdateCalidadCommand request, CancellationToken cancellationToken)
    {
        // Obtener el registro actual para validar cambios
        var currentCalidad = await _calidadRepository.GetByIdAsync(request.Id);
        if (currentCalidad == null)
            throw new BusinessRuleException($"Calidad with ID {request.Id} not found.", 
                "CALIDAD_NOT_FOUND");

        // Si se intenta inactivar, validar que no esté siendo usado por Producto activo
        if (currentCalidad.Activo && request.Activo == false)
        {
            var isUsedByActiveProducto = await _calidadRepository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
            }
        }

        var modificadoPorIdString = _authorizationService.GetCurrentUserId();
        var modificadoPorId = Guid.Parse(modificadoPorIdString);

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
}