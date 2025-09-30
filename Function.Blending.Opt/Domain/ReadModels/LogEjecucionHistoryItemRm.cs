using System;

namespace Function.Blending.Opt.Domain.ReadModels;

/// <summary>
/// Proyección ligera para History (CQRS read model).
/// No es entidad de dominio; se materializa desde EF y se enriquece en el repo.
/// </summary>
public sealed record class LogEjecucionHistoryItemRm
{
  public Guid Id { get; init; }
  public string? Codigo { get; init; }
  public bool? Confirmado { get; init; }
  public Guid EstadoId { get; init; }
  public string? EstadoNombre { get; set; } // se enriquece en el repo
  public DateTime CreadoEl { get; init; } // UTC

  public LogEjecucionHistoryItemRm() { }
}
