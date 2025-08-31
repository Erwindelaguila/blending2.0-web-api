using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetCalidadByIdQuery : BaseQuery<CalidadDTO?>
{
    public Guid Id { get; }

    public GetCalidadByIdQuery(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
