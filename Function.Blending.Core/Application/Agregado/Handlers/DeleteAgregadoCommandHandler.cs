using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class DeleteAgregadoCommandHandler : IRequestHandler<DeleteAgregadoCommand, bool>
{
    private readonly IAgregadoRepository _agregadoRepository;
    public DeleteAgregadoCommandHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }
    public async Task<bool> Handle(DeleteAgregadoCommand request, CancellationToken cancellationToken)
    {
        var agregado = await _agregadoRepository.GetByIdAsync(request.Id);
        if (agregado == null)
            return false;

        try
        {
            // Validación de negocio: verificar si tiene dependencias activas
            // TODO: Implementar validación de dependencias según reglas de negocio
            // Ejemplo: if (await HasActiveDependencies(request.Id))
            //     throw new InvalidOperationException("No se puede eliminar porque tiene dependencias activas.");

            await _agregadoRepository.DeleteAsync(request.Id, request.EliminadoPorId);
            return true;
        }
        catch (Exception ex)
        {
            // TODO: Log la excepción aquí
            // _logger.LogError(ex, "Error al eliminar agregado con ID {Id}", request.Id);
            return false;
        }
    }
}
