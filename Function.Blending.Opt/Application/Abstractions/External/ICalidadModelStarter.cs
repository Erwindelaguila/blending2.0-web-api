using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;

namespace Function.Blending.Opt.Application.Abstractions.External;

public interface ICalidadModelStarter
{
  Task<DispatchResult> StartAsync(CalidadModelPayload payload, CancellationToken ct);
}
