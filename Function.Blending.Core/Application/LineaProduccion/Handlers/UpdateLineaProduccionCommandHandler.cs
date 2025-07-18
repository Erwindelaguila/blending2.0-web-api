using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class UpdateLineaProduccionCommandHandler : IRequestHandler<UpdateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    public UpdateLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<LineaProduccionDTO> Handle(UpdateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var linea = await _lineaProduccionRepository.GetByIdAsync(request.Id);
       
        if (linea == null)

            throw new ArgumentException($"LineaProduccion con ID {request.Id} no encontrada");
        linea.Codigo = request.Codigo ?? linea.Codigo;
        linea.Nombre = request.Nombre ?? linea.Nombre;
        linea.Descripcion = request.Descripcion ?? linea.Descripcion;
        linea.Activo = request.Activo ?? linea.Activo;
        linea.ModificadoPorId = request.ModificadoPorId;
        linea.ModificadoEl = DateTime.Now;

        await _lineaProduccionRepository.UpdateAsync(linea);
        return new LineaProduccionDTO
        {
            Id = linea.Id,
            Codigo = linea.Codigo,
            Nombre = linea.Nombre,
            Descripcion = linea.Descripcion,
            Activo = linea.Activo,
            CreadoPorId = linea.CreadoPorId,
            CreadoEl = linea.CreadoEl,
            ModificadoPorId = linea.ModificadoPorId,
            ModificadoEl = linea.ModificadoEl
        };
    }
}
