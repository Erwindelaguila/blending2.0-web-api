using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

/// <summary>
/// Respuesta estándar para Logística (igual a Calidad en forma):
/// Id, PlantaId, Código, CreadoEl y Estado enriquecido.
/// </summary>
public sealed class LogEjecucionResponse
{
  public Guid Id { get; init; }
  public Guid PlantaId { get; init; }
  public string Codigo { get; init; } = default!;
  public bool? Confirmado { get; init; }
  public DateTime CreadoEl { get; init; }

  // <- ESTA propiedad faltaba y causaba CS1061 en los profiles
  public EstadoResponse? Estado { get; init; }
}
