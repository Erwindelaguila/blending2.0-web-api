using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class DeleteLineaProduccionCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteLineaProduccionCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
