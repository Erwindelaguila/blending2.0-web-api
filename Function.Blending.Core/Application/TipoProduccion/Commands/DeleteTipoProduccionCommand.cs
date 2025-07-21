using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class DeleteTipoProduccionCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteTipoProduccionCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
