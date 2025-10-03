using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

using Function.Blending.Core.Application.Parametro.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class DeleteParametroCommandHandler : IRequestHandler<DeleteParametroCommand, bool>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public DeleteParametroCommandHandler(IParametroRepository parametroRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _parametroRepository = parametroRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<bool> Handle(DeleteParametroCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _parametroRepository.GetByIdAsync(request.Id);
        
        if (parametro == null)
            return false;

        var eliminadoPorId = GetCurrentUserId();

        await _parametroRepository.DeleteAsync(request.Id, eliminadoPorId);
        
        return true;
    }

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(Function.Blending.Core.Shared.Constants.MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is System.Security.Claims.ClaimsPrincipal principal)
            {
                var userIdString = principal.GetUserId();
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
            }
        }
        catch
        {
            // Si hay error obteniendo el usuario, usar fallback
        }
        
        // Fallback: usuario del sistema
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
