using System;

namespace Function.Blending.Opt.Infrastructure.Configuration.Options;

/// <summary>Opciones para el catálogo de estados de Logística.</summary>
public sealed class EstadoLogisticaOptions
{
  /// <summary>Id de la AuxTable que contiene los estados (opcional).</summary>
  public Guid? EstadoTableId { get; set; }

  /// <summary>Si se expone un color desde AuxProp.</summary>
  public bool ExposeColor { get; set; } = false;

  /// <summary>Clave (prop name) en AuxProp para el color (ej. "color").</summary>
  public string? ColorPropClave { get; set; }
}
