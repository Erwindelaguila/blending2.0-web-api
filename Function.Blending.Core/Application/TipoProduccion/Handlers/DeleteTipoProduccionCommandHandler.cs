using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Exceptions;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

/// <summary>
/// Handler para eliminar tipos de producción
/// Incluye auditoría automática y validaciones de negocio
/// </summary>
public class DeleteTipoProduccionCommandHandler : IRequestHandler<DeleteTipoProduccionCommand, bool>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly IAuthorizationService _authorizationService;
    
    public DeleteTipoProduccionCommandHandler(
        ITipoProduccionRepository tipoProduccionRepository,
        IAuthorizationService authorizationService)
    {
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<bool> Handle(DeleteTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        // Obtener usuario actual para auditoría
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
        }

        var tipo = await _tipoProduccionRepository.GetByIdAsync(request.Id);
        if (tipo == null)
            return false;

        try
        {
            // Validar que no esté siendo usado por Producto activo
            var isUsedByActiveProducto = await _tipoProduccionRepository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("el Tipo de Producción", "está siendo usado por al menos un Producto activo");
            }

            await _tipoProduccionRepository.DeleteAsync(request.Id, currentUserId); // Auditoría automática
            return true;
        }
        catch (Exception)
        {
            // TODO: Log la excepción aquí
            // _logger.LogError(ex, "Error al eliminar tipo de producción con ID {Id}", request.Id);
            return false;
        }
    }
}
