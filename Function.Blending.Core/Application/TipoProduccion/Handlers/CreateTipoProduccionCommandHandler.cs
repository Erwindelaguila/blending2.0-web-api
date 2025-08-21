using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class CreateTipoProduccionCommandHandler : IRequestHandler<CreateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    public CreateTipoProduccionCommandHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<TipoProduccionDTO> Handle(CreateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        // Validar que el código no existe
        if (await _tipoProduccionRepository.ExistsActiveCodigoAsync(request.Codigo))
        {
            throw new ArgumentException("El código ya existe", "codigo");
        }

        var tipo = new TipoProduccionEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            LineaProduccionId = request.LineaProduccionId,
            AgregadoId = request.AgregadoId,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };
        await _tipoProduccionRepository.CreateAsync(tipo);
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
