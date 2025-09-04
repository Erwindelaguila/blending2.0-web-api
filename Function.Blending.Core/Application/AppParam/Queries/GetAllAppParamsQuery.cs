using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Queries;

public class GetAllAppParamsQuery : IRequest<AppParamResponseDTO>
{
    public int Page { get; }
    public int Size { get; }
    public AppParamFilterDTO? Filters { get; }
    public bool IsGlobal { get; }

    public GetAllAppParamsQuery(int page, int size, AppParamFilterDTO? filters = null, bool isGlobal = false)
    {
        Page = page;
        Size = size;
        Filters = filters;
        IsGlobal = isGlobal;
    }
}
