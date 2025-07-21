using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;


namespace Function.Blending.Core.Application.Calidad.Handlers;

public class UpdateCalidadCommandHandler : IRequestHandler<UpdateCalidadCommand, CalidadDTO>
{
    private readonly ICalidadRepository _repository;

    public UpdateCalidadCommandHandler(ICalidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<CalidadDTO> Handle(UpdateCalidadCommand request, CancellationToken cancellationToken)
    {
        var calidad =  await _repository.GetByIdAsync(request.Id);

        if (calidad == null)
            throw new ArgumentException($"Calidad con ID {request.Id} no encontrada");

        // Actualiza solo los campos necesarios
        calidad.Codigo = request.Codigo ?? calidad.Codigo;
        calidad.Nombre = request.Nombre ?? calidad.Nombre;
        calidad.CodigoMaterial = request.CodigoMaterial ?? calidad.CodigoMaterial;
        calidad.Descripcion = request.Descripcion ?? calidad.Descripcion;
        calidad.NoConforme = request.NoConforme ?? calidad.NoConforme;
        calidad.Activo = request.Activo ?? calidad.Activo;
        calidad.ModificadoPorId = request.ModificadoPorId;
        calidad.ModificadoEl = DateTime.Now;

        await _repository.UpdateAsync(calidad);

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
            ModificadoEl = calidad.ModificadoEl,
        };
    }
}