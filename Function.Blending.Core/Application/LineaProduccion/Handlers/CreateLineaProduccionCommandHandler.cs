using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class CreateLineaProduccionCommandHandler : IRequestHandler<CreateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    public CreateLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<LineaProduccionDTO> Handle(CreateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var linea = new LineaProduccionEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
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
            CreadoPorId = linea.CreadoPorId,
            CreadoEl = linea.CreadoEl
        };
    }
}
