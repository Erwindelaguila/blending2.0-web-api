using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Queries;

public class GetAllAppParamsQuery : IRequest<PagedResponse<AppParamDTO>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Key { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? Fecha { get; set; }
}
