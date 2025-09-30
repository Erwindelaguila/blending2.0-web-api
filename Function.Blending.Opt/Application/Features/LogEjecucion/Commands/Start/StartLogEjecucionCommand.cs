using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start;

// Igual patrón que Calidad: record + props init adicionales
public sealed partial record StartLogEjecucionCommand(Guid CreadoPorId) : IRequest<Result<StartLogEjecucionResponse>>
{
  public LogisticaStartPayload Start { get; init; } = null!;
  public LogisticaModelPayload Model { get; init; } = null!;
}
