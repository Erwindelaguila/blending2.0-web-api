using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class CreateCalidadCommandHandler : IRequestHandler<CreateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _calidadRepository;

    public CreateCalidadCommandHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<CalidadDTO> Handle(CreateCalidadCommand request, CancellationToken cancellationToken)
    {
        var calidad = new CalidadEntity()
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            CodigoMaterial = request.CodigoMaterial,
            Descripcion = request.Descripcion,
            NoConforme = request.NoConforme ?? false,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
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
}