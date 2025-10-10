using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class DeleteLineaProduccionCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteLineaProduccionCommand(Guid id)
    {
        Id = id;
    }
}
