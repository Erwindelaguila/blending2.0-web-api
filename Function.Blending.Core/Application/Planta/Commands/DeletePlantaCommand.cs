using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Planta.Commands;

public class DeletePlantaCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    [JsonConstructor]
    public DeletePlantaCommand(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
