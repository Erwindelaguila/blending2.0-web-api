using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetCalidadByIdQuery : BaseQuery<CalidadDTO?>
{
    public Guid Id { get; }

    public GetCalidadByIdQuery(Guid id)
    {
        Id = id;
    }
}
