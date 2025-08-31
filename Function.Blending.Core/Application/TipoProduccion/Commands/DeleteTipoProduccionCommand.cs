using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

/// <summary>
/// Command para eliminar un tipo de producción
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class DeleteTipoProduccionCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteTipoProduccionCommand(Guid id, object requestContext) : base(requestContext)
    {
        Id = id;
    }
}
