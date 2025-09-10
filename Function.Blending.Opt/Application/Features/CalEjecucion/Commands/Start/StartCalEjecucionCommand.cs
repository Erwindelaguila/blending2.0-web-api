using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;

public sealed partial record StartCalEjecucionCommand(
  Guid PlantaId,
  string? Mensaje,
  Guid CreadoPorId
) : IRequest<Result<StartCalEjecucionResponse>>
{
  public CalInpFiltroDto? Filtro { get; init; }                        // NUEVO (opcional)
  public IReadOnlyList<CalInpParametroDto>? Parametros { get; init; }  // NUEVO (opcional)
}
