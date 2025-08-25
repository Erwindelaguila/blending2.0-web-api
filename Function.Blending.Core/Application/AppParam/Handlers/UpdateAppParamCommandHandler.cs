using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class UpdateAppParamCommandHandler : IRequestHandler<UpdateAppParamCommand, object>
{
    private readonly IAppParamRepository _appParamRepository;

    public UpdateAppParamCommandHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<object> Handle(UpdateAppParamCommand request, CancellationToken cancellationToken)
    {
        var existingAppParam = await _appParamRepository.GetByKeyAsync(request.Key);
        
        if (existingAppParam == null)
        {
            throw new KeyNotFoundException($"AppParam with key '{request.Key}' not found");
        }

        existingAppParam.Value = request.Value;
        existingAppParam.Description = request.Description;
        // Solo actualizar category y group si vienen en el request (no son null)
        if (request.Category != null)
            existingAppParam.Category = request.Category;
        if (request.Group != null)
            existingAppParam.Group = request.Group;
        existingAppParam.IsActive = request.IsActive ?? existingAppParam.IsActive;
        // Los campos internos no se modifican desde el frontend
        existingAppParam.IsInternal = request.IsInternal ?? existingAppParam.IsInternal;
        existingAppParam.IsVisible = request.IsVisible ?? existingAppParam.IsVisible;
        existingAppParam.IsDisableable = request.IsDisableable ?? existingAppParam.IsDisableable;
        existingAppParam.IsRemovable = request.IsRemovable ?? existingAppParam.IsRemovable;
        existingAppParam.ModificadoPorId = request.ModificadoPorId;
        existingAppParam.ModificadoEl = DateTime.UtcNow;

        var updatedAppParam = await _appParamRepository.UpdateAndReturnAsync(existingAppParam);

        return new AppParamDTO
        {
            Key = updatedAppParam.Key,
            Value = updatedAppParam.Value,
            Description = updatedAppParam.Description,
            Category = updatedAppParam.Category,
            Group = updatedAppParam.Group,
            IsActive = updatedAppParam.IsActive,
            IsInternal = updatedAppParam.IsInternal,
            IsVisible = updatedAppParam.IsVisible,
            IsDisableable = updatedAppParam.IsDisableable,
            IsRemovable = updatedAppParam.IsRemovable,
            CreadoPorId = updatedAppParam.CreadoPorId,
            CreadoEl = updatedAppParam.CreadoEl,
            ModificadoPorId = updatedAppParam.ModificadoPorId,
            ModificadoEl = updatedAppParam.ModificadoEl
        };
    }
}
