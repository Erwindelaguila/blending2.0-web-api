using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class CreateAppParamCommandHandler : IRequestHandler<CreateAppParamCommand, object>
{
    private readonly IAppParamRepository _appParamRepository;

    public CreateAppParamCommandHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<object> Handle(CreateAppParamCommand request, CancellationToken cancellationToken)
    {
        var appParam = new AppParamEntity
        {
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            Category = request.Category,
            Group = request.Group,
            IsActive = request.IsActive ?? true,
            IsInternal = request.IsInternal ?? false,
            IsVisible = request.IsVisible ?? true,
            IsDisableable = request.IsDisableable ?? true,
            IsRemovable = request.IsRemovable ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };

        await _appParamRepository.CreateAsync(appParam);

        return new AppParamDTO
        {
            Key = appParam.Key,
            Value = appParam.Value,
            Description = appParam.Description,
            Category = appParam.Category,
            Group = appParam.Group,
            IsActive = appParam.IsActive,
            IsInternal = appParam.IsInternal,
            IsVisible = appParam.IsVisible,
            IsDisableable = appParam.IsDisableable,
            IsRemovable = appParam.IsRemovable,
            CreadoPorId = appParam.CreadoPorId,
            CreadoEl = appParam.CreadoEl,
            ModificadoPorId = appParam.ModificadoPorId,
            ModificadoEl = appParam.ModificadoEl
        };
    }
}
