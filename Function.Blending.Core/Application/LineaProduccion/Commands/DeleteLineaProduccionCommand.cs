using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class DeleteLineaProduccionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
