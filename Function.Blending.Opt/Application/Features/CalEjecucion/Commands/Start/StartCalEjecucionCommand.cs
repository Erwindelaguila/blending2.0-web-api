using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;

public sealed partial record StartCalEjecucionCommand : IRequest<Result<StartCalEjecucionResponse>>
{
  public Guid CreadoPorId { get; init; } = default;
  public CalidadStartPayload Start { get; init; } = null!;
  public CalidadModelPayload Model { get; init; } = null!;
}
