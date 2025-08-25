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

        if (!existingAppParam.IsRemovable)
        {
            throw new InvalidOperationException($"AppParam with key '{request.Key}' is not removable");
        }

        await _appParamRepository.DeleteAsync(request.Key);
        
        return new { success = true, message = $"AppParam with key '{request.Key}' deleted successfully" };
    }
}
