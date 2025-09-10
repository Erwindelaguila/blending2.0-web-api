using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ToggleState;

public sealed record ToggleCalEstadoCommand(
  Guid Id,
  Guid ModificadoPorId
) : IRequest<Result<CalEjecucionResponse>>;
