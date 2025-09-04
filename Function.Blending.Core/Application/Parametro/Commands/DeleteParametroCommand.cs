using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class DeleteParametroCommand : BaseCommand<bool>
{
    public Guid Id { get; }

    public DeleteParametroCommand(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
