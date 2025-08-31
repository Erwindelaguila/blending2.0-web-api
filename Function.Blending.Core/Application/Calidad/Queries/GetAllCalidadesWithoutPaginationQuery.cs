using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesWithoutPaginationQuery : BaseQuery<List<CalidadDTO>>
{
    public GetAllCalidadesWithoutPaginationQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
