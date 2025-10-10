using System;
using System.Collections.Generic;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Complete;

public sealed record CompleteCalEjecucionCommand(
  Guid Id,
  Guid EstadoId,
  string? Mensaje,
  Guid ModificadoPorId,
  IReadOnlyList<CalOutResumenDto>? Resumenes,
  IReadOnlyList<CalOutDetalleDto>? Detalles
) : IRequest<Result<CalEjecucionResponse>>;
