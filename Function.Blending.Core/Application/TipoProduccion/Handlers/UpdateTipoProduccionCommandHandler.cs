using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class UpdateTipoProduccionCommandHandler : IRequestHandler<UpdateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    public UpdateTipoProduccionCommandHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<TipoProduccionDTO> Handle(UpdateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        // Crear entidad con los nuevos datos
        var tipoToUpdate = new TipoProduccionEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            LineaProduccionId = request.LineaProduccionId,
            AgregadoId = request.AgregadoId,
            Activo = request.Activo ?? true,
            ModificadoPorId = request.ModificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var updatedTipo = await _tipoProduccionRepository.UpdateAndReturnAsync(tipoToUpdate);

        return new TipoProduccionDTO
        {
            Id = updatedTipo.Id,
            Codigo = updatedTipo.Codigo,
            Nombre = updatedTipo.Nombre,
            Descripcion = updatedTipo.Descripcion,
            LineaProduccionId = updatedTipo.LineaProduccionId,
            AgregadoId = updatedTipo.AgregadoId,
            Activo = updatedTipo.Activo,
            CreadoPorId = updatedTipo.CreadoPorId,
            CreadoEl = updatedTipo.CreadoEl,
            ModificadoPorId = updatedTipo.ModificadoPorId,
            ModificadoEl = updatedTipo.ModificadoEl
        };
    }
}
