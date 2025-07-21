using MediatR;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class DeleteParametroCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteParametroCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
