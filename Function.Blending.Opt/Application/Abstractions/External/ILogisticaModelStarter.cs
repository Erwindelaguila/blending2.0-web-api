using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

namespace Function.Blending.Opt.Application.Abstractions.External;

public interface ILogisticaModelStarter
{
  Task<DispatchResult> StartAsync(LogisticaModelPayload payload, CancellationToken ct);
}
