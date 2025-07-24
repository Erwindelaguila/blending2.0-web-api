using MediatR;


namespace Function.Blending.Core.Application.Agregado.Commands;

public class DeleteAgregadoCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteAgregadoCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
