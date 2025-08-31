using Function.Blending.Core.Application.Common.Queries;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Queries;

public class GetLineaProduccionByIdQuery : BaseQuery<LineaProduccionDTO?>
{
    public Guid Id { get; }

    public GetLineaProduccionByIdQuery(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
