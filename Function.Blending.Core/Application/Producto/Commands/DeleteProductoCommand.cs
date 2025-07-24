using MediatR;

namespace Function.Blending.Core.Application.Producto.Commands;

public class DeleteProductoCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteProductoCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
