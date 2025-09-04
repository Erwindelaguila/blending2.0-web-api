using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.AppParam.Commands;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;


public class DeleteAppParamCommandHandler : IRequestHandler<DeleteAppParamCommand, bool>
{
    private readonly IAppParamRepository _appParamRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteAppParamCommandHandler(
        IAppParamRepository appParamRepository,
        IAuthorizationService authorizationService)
    {
        _appParamRepository = appParamRepository ?? throw new ArgumentNullException(nameof(appParamRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<bool> Handle(DeleteAppParamCommand request, CancellationToken cancellationToken)
    {

        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
        }

        var existingAppParam = await _appParamRepository.GetByKeyAsync(request.Key);
        
        if (existingAppParam == null)
            return false;

   
        if (!existingAppParam.IsRemovable)
        {
            throw new InvalidOperationException($"No se puede eliminar el parámetro '{request.Key}' porque no es removible del sistema.");
        }

        try
        {
            await _appParamRepository.DeleteAsync(request.Key);
            return true;
        }
        catch (Exception)
        {

            return false;
        }
    }
}
