using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class GetAppParamByKeyQueryHandler : IRequestHandler<GetAppParamByKeyQuery, AppParamDTO?>
{
    private readonly IAppParamRepository _appParamRepository;

    public GetAppParamByKeyQueryHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<AppParamDTO?> Handle(GetAppParamByKeyQuery request, CancellationToken cancellationToken)
    {
        var appParam = await _appParamRepository.GetByKeyAsync(request.Key);
        
        if (appParam == null)
            return null;

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
