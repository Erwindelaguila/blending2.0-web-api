using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Agregado.Queries;

public class GetAllAgregadosActivosQuery : BaseQuery<List<AgregadoActivoDTO>>
{
    public GetAllAgregadosActivosQuery(HttpRequestData? requestContext = null) : base(requestContext!)
    {
    }
}
