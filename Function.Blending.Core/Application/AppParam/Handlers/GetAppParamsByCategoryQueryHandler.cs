using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class GetAppParamsByCategoryQueryHandler : IRequestHandler<GetAppParamsByCategoryQuery, IEnumerable<AppParamDTO>>
{
    private readonly IAppParamRepository _appParamRepository;

    public GetAppParamsByCategoryQueryHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<IEnumerable<AppParamDTO>> Handle(GetAppParamsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var appParams = await _appParamRepository.GetByCategoryAsync(request.Category);
        
        return appParams.Select(ap => new AppParamDTO
        {
            Key = ap.Key,
            Value = ap.Value,
            Description = ap.Description,
            Category = ap.Category,
            Group = ap.Group,
            IsActive = ap.IsActive,
            IsInternal = ap.IsInternal,
            IsVisible = ap.IsVisible,
            IsDisableable = ap.IsDisableable,
            IsRemovable = ap.IsRemovable,
            CreadoPorId = ap.CreadoPorId,
            CreadoEl = ap.CreadoEl,
            ModificadoPorId = ap.ModificadoPorId,
            ModificadoEl = ap.ModificadoEl
        });
    }
}
