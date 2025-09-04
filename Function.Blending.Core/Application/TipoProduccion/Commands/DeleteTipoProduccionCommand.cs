using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;


public class DeleteTipoProduccionCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteTipoProduccionCommand(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
