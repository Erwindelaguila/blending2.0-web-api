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

        await _tipoProduccionRepository.DeleteAsync(request.Id);
        
        return true;
    }
}
