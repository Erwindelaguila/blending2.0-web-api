using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.Commands;
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

        await _lineaProduccionRepository.DeleteAsync(request.Id);
        
        return true;
    }
}
