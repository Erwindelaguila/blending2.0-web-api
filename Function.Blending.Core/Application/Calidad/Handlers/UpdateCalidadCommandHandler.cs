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
        var calidadExit =  await _repository.GetByIdAsync(request.Id);

        if (calidadExit == null)
        {
            throw new NullReferenceException($"La calidad con el id: {request.Id} no existe");
        }
        
        // Actualiza solo los campos necesarios
        calidadExit.Codigo = request.Codigo ?? calidadExit.Codigo;
        calidadExit.Nombre = request.Nombre ?? calidadExit.Nombre;
        calidadExit.CodigoMaterial = request.CodigoMaterial ?? calidadExit.CodigoMaterial;
        calidadExit.Descripcion = request.Descripcion ?? calidadExit.Descripcion;
        calidadExit.NoConforme = request.NoConforme ?? calidadExit.NoConforme;
        calidadExit.Activo = request.Activo ?? calidadExit.Activo;
        calidadExit.ModificadoPorId = request.ModificadoPorId;
        calidadExit.ModificadoEl = DateTime.Now;
        
        
        await _repository.UpdateAsync(calidadExit);
        
        return new CalidadDTO
        {
            Id = calidadExit.Id,
            Codigo = calidadExit.Codigo,
            Nombre = calidadExit.Nombre,
            CodigoMaterial = calidadExit.CodigoMaterial,
            Descripcion = calidadExit.Descripcion,
            NoConforme = calidadExit.NoConforme,
            Activo = calidadExit.Activo,
            CreadoPorId = calidadExit.CreadoPorId,
            CreadoEl = calidadExit.CreadoEl,
            ModificadoPorId = calidadExit.ModificadoPorId,
            ModificadoEl = calidadExit.ModificadoEl,
        };
    }
}