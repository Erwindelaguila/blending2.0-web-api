namespace Function.Blending.Opt.Infrastructure.Configuration.Options;

/// <summary>
/// Opciones funcionales del catálogo de <b>Estados de Calidad</b>:
/// - Qué tabla AuxTable es la válida (TableId)
/// - Qué metadatos exponer (p.ej. color) y bajo qué clave
/// </summary>
public sealed class EstadoCalidadOptions
{
  /// <summary>TableId (Guid) de la AuxTable que contiene los estados (opcional; si no se define, no se valida).</summary>
  public Guid? EstadoTableId { get; set; }

  /// <summary>Si true, expone metadatos (p.ej., color) en EstadoRef/EstadoResponse en GetById/Start.</summary>
  public bool ExposeColor { get; set; } = true;

  /// <summary>Clave de propiedad (AuxProp.Clave) que contiene el color (default "color").</summary>
  public string ColorPropClave { get; set; } = "color";
}
