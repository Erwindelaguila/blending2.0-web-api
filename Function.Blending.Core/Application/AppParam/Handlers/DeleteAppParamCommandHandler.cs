using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Commands;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class DeleteAppParamCommandHandler : IRequestHandler<DeleteAppParamCommand, object>
{
    private readonly IAppParamRepository _appParamRepository;

    public DeleteAppParamCommandHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<object> Handle(DeleteAppParamCommand request, CancellationToken cancellationToken)
    {
        var existingAppParam = await _appParamRepository.GetByKeyAsync(request.Key);
        
        if (existingAppParam == null)
        {
            throw new KeyNotFoundException($"AppParam with key '{request.Key}' not found");
        }

        // VALIDAR: Solo se puede eliminar si isRemovable = true
        if (!existingAppParam.IsRemovable)
        {
            throw new InvalidOperationException($"Cannot delete parameter '{request.Key}' because isRemovable = false. This parameter cannot be removed from the system.");
        }

        await _appParamRepository.DeleteAsync(request.Key);

        return new { message = $"AppParam '{request.Key}' deleted successfully" };
    }
}
