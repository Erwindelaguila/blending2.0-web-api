using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class DeleteLineaProduccionCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteLineaProduccionCommand(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
