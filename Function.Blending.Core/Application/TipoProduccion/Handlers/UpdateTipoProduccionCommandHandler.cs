using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;

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
        var tipo = await _tipoProduccionRepository.GetByIdAsync(request.Id);
        if (tipo == null)
            throw new ArgumentException($"TipoProduccion con ID {request.Id} no encontrado");
        tipo.Codigo = request.Codigo;
        tipo.Nombre = request.Nombre;
        tipo.Descripcion = request.Descripcion;
        tipo.LineaProduccionId = request.LineaProduccionId;
        tipo.AgregadoId = request.AgregadoId;
        tipo.Activo = request.Activo;
        tipo.ModificadoPorId = request.ModificadoPorId;
        tipo.ModificadoEl = DateTime.Now;
       
        await _tipoProduccionRepository.UpdateAsync(tipo);
       
        return new TipoProduccionDTO
        {
            Id = tipo.Id,
            Codigo = tipo.Codigo,
            Nombre = tipo.Nombre,
            Descripcion = tipo.Descripcion,
            LineaProduccionId = tipo.LineaProduccionId,
            AgregadoId = tipo.AgregadoId,
            Activo = tipo.Activo,
            CreadoPorId = tipo.CreadoPorId,
            CreadoEl = tipo.CreadoEl,
            ModificadoPorId = tipo.ModificadoPorId,
            ModificadoEl = tipo.ModificadoEl
        };
    }
}
