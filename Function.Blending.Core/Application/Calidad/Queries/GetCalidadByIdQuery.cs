using Function.Blending.Core.Application.Calidad.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Queries;

public class GetCalidadByIdQuery : IRequest<CalidadDTO>
{
    public Guid Id { get; }

    public GetCalidadByIdQuery(Guid id)
    {
        Id = id;
    }
}
