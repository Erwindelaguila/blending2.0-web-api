using Function.Blending.Core.Application.Interfaces.Repositories;
// using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class DeleteCalidadCommandHandler : IRequestHandler<DeleteCalidadCommand, bool>
{
    private readonly ICalidadRepository _calidadRepository;
    // private readonly IAuthorizationService _authorizationService;

    public DeleteCalidadCommandHandler(ICalidadRepository calidadRepository/*, IAuthorizationService authorizationService*/)
    {
        _calidadRepository = calidadRepository;
        // _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteCalidadCommand request, CancellationToken cancellationToken)
    {
        var calidad = await _calidadRepository.GetByIdAsync(request.Id);
        
        if (calidad == null)
            return false;

        var isUsedByActiveProducto = await _calidadRepository.IsUsedByActiveProductoAsync(request.Id);
        if (isUsedByActiveProducto)
        {
            throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
        }

        // var eliminadoPorIdString = _authorizationService.GetCurrentUserId();
        // var eliminadoPorId = Guid.Parse(eliminadoPorIdString);
        var eliminadoPorId = Guid.NewGuid(); // Valor temporal mientras no hay autorización

        await _calidadRepository.DeleteAsync(request.Id, eliminadoPorId);
        
        return true;
    }
}
