using System;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start
{
  // Igual patrón que Calidad: record + props init adicionales
  public sealed partial record StartLogEjecucionCommand(
    string? Mensaje,
    Guid CreadoPorId
  ) : IRequest<Result<StartLogEjecucionResponse>>
  {
    public LogInpInfoDto? Info { get; init; }
    public LogInpFiltroDto? Filtro { get; init; }
    public LogInpOfertaDto? Oferta { get; init; }
  }
}
