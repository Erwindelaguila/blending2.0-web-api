using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Queries;

public class GetParametroByIdQuery : IRequest<ParametroDTO?>
{
    public Guid Id { get; }

    public GetParametroByIdQuery(Guid id)
    {
        Id = id;
    }
}
