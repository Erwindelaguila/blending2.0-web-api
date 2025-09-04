using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class CreateLineaProduccionCommandHandler : IRequestHandler<CreateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAuthorizationService _authorizationService;

    public CreateLineaProduccionCommandHandler(
        ILineaProduccionRepository lineaProduccionRepository,
        IAuthorizationService authorizationService)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _authorizationService = authorizationService;
    }

    public async Task<LineaProduccionDTO> Handle(CreateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new InvalidOperationException("User ID inválido en headers");
        }

        var linea = new LineaProduccionEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            CreadoPorId = currentUserId,
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
