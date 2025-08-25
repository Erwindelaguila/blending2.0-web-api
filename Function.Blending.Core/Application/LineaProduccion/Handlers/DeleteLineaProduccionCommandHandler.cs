using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class DeleteLineaProduccionCommandHandler : IRequestHandler<DeleteLineaProduccionCommand, bool>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public DeleteLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<bool> Handle(DeleteLineaProduccionCommand request, CancellationToken cancellationToken)
    {
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
            await _lineaProduccionRepository.DeleteAsync(request.Id, request.EliminadoPorId);
            return true;
        }
        catch (Exception)
        {
            // Si falla por constraint de BD, lanzar excepción más específica
            throw new EntityInUseException("la Línea de Producción", "tiene dependencias en la base de datos");
        }
    }
}
