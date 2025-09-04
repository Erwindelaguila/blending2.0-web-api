using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetParametroByIdQuery : BaseQuery<ParametroDTO?>
{
    public Guid Id { get; }

    public GetParametroByIdQuery(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
