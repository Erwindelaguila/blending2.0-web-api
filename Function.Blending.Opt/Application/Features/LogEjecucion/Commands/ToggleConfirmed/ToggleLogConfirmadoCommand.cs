using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.ToggleState;

public sealed record ToggleLogConfirmadoCommand(
  Guid Id,
  Guid ModificadoPorId
) : IRequest<Result<LogEjecucionResponse>>;
