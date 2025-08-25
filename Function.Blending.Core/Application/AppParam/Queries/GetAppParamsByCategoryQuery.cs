using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Queries;

public class GetAppParamsByCategoryQuery : IRequest<IEnumerable<AppParamDTO>>
{
    public string Category { get; }

    public GetAppParamsByCategoryQuery(string category)
    {
        Category = category;
    }
}
