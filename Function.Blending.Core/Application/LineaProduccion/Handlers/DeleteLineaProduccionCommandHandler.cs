using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class DeleteLineaProduccionCommandHandler : IRequestHandler<DeleteLineaProduccionCommand, bool>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteLineaProduccionCommandHandler(
        ILineaProduccionRepository lineaProduccionRepository,
        IAuthorizationService authorizationService)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        // Obtener User ID desde el contexto de autorización
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new InvalidOperationException("User ID inválido en headers");
        }

        var linea = await _lineaProduccionRepository.GetByIdAsync(request.Id);
        if (linea == null)
            return false;

        // Validación de regla de negocio: no se puede eliminar si está siendo usado por TipoProducción activo
        var isUsedByActiveTipoProduccion = await _lineaProduccionRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
        if (isUsedByActiveTipoProduccion)
        {
            throw new EntityInUseException("la Línea de Producción", "está siendo usada por al menos un Tipo de Producción activo");
        }

        try
        {
            await _lineaProduccionRepository.DeleteAsync(request.Id, currentUserId);
            return true;
        }
        catch (Exception)
        {
            // Si falla por constraint de BD, lanzar excepción más específica
            throw new EntityInUseException("la Línea de Producción", "tiene dependencias en la base de datos");
        }
    }
}
