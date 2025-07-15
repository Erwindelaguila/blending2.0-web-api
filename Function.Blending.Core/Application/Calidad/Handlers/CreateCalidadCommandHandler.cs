using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class CreateCalidadCommandHandler :  IRequestHandler<CreateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _repository;

    public CreateCalidadCommandHandler(ICalidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<CalidadDTO> Handle(CreateCalidadCommand request, CancellationToken cancellationToken)
    {
        var Calidad = new CalidadEntity()
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            CodigoMaterial = request.CodigoMaterial,
            Descripcion = request.Descripcion,
            NoConforme = request.NoConforme?? false,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.Now
        };
        
        await _repository.CreateAsync(Calidad);

        return new CalidadDTO
        {
            Id = Calidad.Id,
            Codigo = Calidad.Codigo,
            Nombre = Calidad.Nombre,
            CodigoMaterial = Calidad.CodigoMaterial,
            Descripcion = Calidad.Descripcion,
            NoConforme = Calidad.NoConforme,
            Activo = Calidad.Activo,
            CreadoPorId = Calidad.CreadoPorId,
            CreadoEl = Calidad.CreadoEl,
            ModificadoPorId = Calidad.ModificadoPorId,
            ModificadoEl = Calidad.ModificadoEl
        };

    }
}