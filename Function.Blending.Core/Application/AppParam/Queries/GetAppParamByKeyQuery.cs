using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Queries;

public class GetAppParamByKeyQuery : IRequest<AppParamDTO?>
{
    public string Key { get; }

    public GetAppParamByKeyQuery(string key)
    {
        Key = key;
    }
}
