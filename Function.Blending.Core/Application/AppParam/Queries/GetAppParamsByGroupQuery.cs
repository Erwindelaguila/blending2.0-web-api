using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Queries;

public class GetAppParamsByGroupQuery : IRequest<IEnumerable<AppParamDTO>>
{
    public string Group { get; }

    public GetAppParamsByGroupQuery(string group)
    {
        Group = group;
    }
}
