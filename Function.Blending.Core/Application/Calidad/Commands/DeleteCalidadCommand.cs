using MediatR;
namespace Function.Blending.Core.Application.Calidad.Commands;

public class DeleteCalidadCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid EliminadoPorId { get; }

    public DeleteCalidadCommand(Guid id, Guid eliminadoPorId)
    {
        Id = id;
        EliminadoPorId = eliminadoPorId;
    }
}
