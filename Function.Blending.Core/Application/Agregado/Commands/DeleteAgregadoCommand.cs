using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Agregado.Commands;

public class DeleteAgregadoCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteAgregadoCommand(Guid id)
    {
        Id = id;
    }
}
