using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Agregado.Queries;

public class GetAgregadoByIdQuery : BaseQuery<AgregadoDTO?>
{
    public Guid Id { get; }

    public GetAgregadoByIdQuery(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
