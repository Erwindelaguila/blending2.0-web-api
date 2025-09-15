using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using System;

namespace Function.Blending.Opt.Domain.Entities;

/// <summary>
/// Entidad de dominio mínima para Calidad. 
/// Mantiene compatibilidad con contratos existentes (EstadoId/EstadoNombre)
/// y añade una referencia compuesta al estado (Estado) para CQRS rico.
/// </summary>
public sealed class CalEjecucion
{
  public EjecucionId Id { get; set; }

  public PlantaId PlantaId { get; private set; }
  public EstadoId EstadoId { get; private set; }

  public string? Codigo { get; set; }
  public string? Mensaje { get; set; }
  public DateTime CreadoEl { get; set; } // UTC

  /// <summary>Nombre legible del estado (AuxRow.Nombre) correspondiente a EstadoId.</summary>
  public string? EstadoNombre { get; set; }

  /// <summary>
  /// Referencia compuesta al estado (VO). Se poblará progresivamente en pasos siguientes.
  /// No sustituye aún a EstadoId/EstadoNombre para no romper contratos existentes.
  /// </summary>
  public EstadoCalidadRef? Estado { get; set; }

  public IReadOnlyList<CalOutResumenRef>? Resumenes { get; set; }

  public CalEjecucion() { }
  public CalEjecucion(EjecucionId id) => Id = id;
}
