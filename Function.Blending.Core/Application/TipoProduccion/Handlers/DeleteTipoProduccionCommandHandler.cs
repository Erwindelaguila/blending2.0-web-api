using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Common.Exceptions;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class DeleteTipoProduccionCommandHandler : IRequestHandler<DeleteTipoProduccionCommand, bool>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    public DeleteTipoProduccionCommandHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<bool> Handle(DeleteTipoProduccionCommand request, CancellationToken cancellationToken)
    {
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

            await _tipoProduccionRepository.DeleteAsync(request.Id, request.EliminadoPorId);
            return true;
        }
        catch (Exception ex)
        {
            // TODO: Log la excepción aquí
            // _logger.LogError(ex, "Error al eliminar tipo de producción con ID {Id}", request.Id);
            return false;
        }
    }
}
