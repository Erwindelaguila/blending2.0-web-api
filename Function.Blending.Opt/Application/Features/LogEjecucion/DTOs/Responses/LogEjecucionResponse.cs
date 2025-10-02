using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Output;
using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

/// <summary>
/// Respuesta estándar para Logística (igual a Calidad en forma):
/// Id, Código, CreadoEl y Estado enriquecido.
/// </summary>
public sealed class LogEjecucionResponse
{
  public Guid Id { get; init; }
  public string Codigo { get; init; } = default!;
  public bool? Confirmado { get; init; }
  public DateTime CreadoEl { get; init; }

  public EstadoResponse? Estado { get; init; }

  public LogInpInfoDto? Info { get; set; }
  public LogInpOfertaDto? Oferta { get; set; }
  public LogInpFiltroDto? Filtro { get; set; }
  public IReadOnlyList<LogOutContenedorDto>? Contenedores { get; set; }
}
