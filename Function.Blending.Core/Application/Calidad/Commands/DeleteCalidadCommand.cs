using Function.Blending.Core.Application.Common.Commands;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Calidad.Commands;

public class DeleteCalidadCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    [JsonConstructor]
    public DeleteCalidadCommand(Guid id)
    {
        Id = id;
    }
}
