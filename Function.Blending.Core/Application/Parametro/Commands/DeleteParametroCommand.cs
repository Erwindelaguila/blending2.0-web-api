using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class DeleteParametroCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteParametroCommand(Guid id)
    {
        Id = id;
    }
}
