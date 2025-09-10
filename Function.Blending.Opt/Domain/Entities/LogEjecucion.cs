using System;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Domain.Entities;

public sealed class LogEjecucion
{
  public EjecucionId Id { get; set; }

  public PlantaId PlantaId { get; private set; }
  public EstadoId EstadoId { get; private set; }

  public string? Codigo { get; set; }
  public bool? Confirmado { get; set; }
  public DateTime CreadoEl { get; set; }   // UTC

  public string? EstadoNombre { get; set; }
  public EstadoLogisticaRef? Estado { get; set; }

  public string? Mensaje { get; set; }

  public LogEjecucion() { }
  public LogEjecucion(EjecucionId id) => Id = id;
}
