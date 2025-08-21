using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Commands;

public class DeleteProductoCommand : IRequest<BaseResponse<object>>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteProductoCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
