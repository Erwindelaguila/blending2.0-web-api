using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetParametroByIdQuery : BaseQuery<ParametroDTO?>
{
    public Guid Id { get; }

    public GetParametroByIdQuery(Guid id)
    {
        Id = id;
    }
}
