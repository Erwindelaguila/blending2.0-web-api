using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;

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
            // Validación de negocio: verificar si tiene dependencias activas
            // TODO: Implementar validación de dependencias según reglas de negocio
            // Ejemplo: if (await HasActiveDependencies(request.Id))
            //     throw new InvalidOperationException("No se puede eliminar porque tiene dependencias activas.");

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
