using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetAllCalidadesActivasQuery : BaseQuery<List<CalidadActivaDTO>>
{
    public GetAllCalidadesActivasQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
