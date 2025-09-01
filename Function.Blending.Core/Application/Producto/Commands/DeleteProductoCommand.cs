using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Producto.Commands;


public class DeleteProductoCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteProductoCommand(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
