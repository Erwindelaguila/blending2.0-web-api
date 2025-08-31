using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Producto.Commands;

/// <summary>
/// Command para eliminar un producto
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class DeleteProductoCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteProductoCommand(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
