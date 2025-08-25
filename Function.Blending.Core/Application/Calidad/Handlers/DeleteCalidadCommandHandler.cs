using MediatR;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Common.Exceptions;

public class DeleteCalidadCommandHandler : IRequestHandler<DeleteCalidadCommand, bool>
{
    private readonly ICalidadRepository _calidadRepository;

    public DeleteCalidadCommandHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<bool> Handle(DeleteCalidadCommand request, CancellationToken cancellationToken)
    {
        var entity = await _calidadRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return false;

        // Validar que no esté siendo usado por Producto activo
        var isUsedByActiveProducto = await _calidadRepository.IsUsedByActiveProductoAsync(request.Id);
        if (isUsedByActiveProducto)
        {
            throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
        }
            
        await _calidadRepository.DeleteAsync(request.Id, request.EliminadoPorId);
        return true;
    }
}
