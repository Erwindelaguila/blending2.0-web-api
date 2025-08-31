using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetAllParametrosWithoutPaginationQuery : BaseQuery<List<ParametroDTO>>
{
    public GetAllParametrosWithoutPaginationQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
