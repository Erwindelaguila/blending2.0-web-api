using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class DeleteTipoProduccionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
