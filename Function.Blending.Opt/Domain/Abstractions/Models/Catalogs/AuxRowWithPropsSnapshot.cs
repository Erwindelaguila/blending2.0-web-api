using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.Abstractions.Models.Catalogs
{
  /// <summary>
  /// Snapshot de un registro Aux (row) con un subconjunto de propiedades (clave → valor).
  /// </summary>
  public sealed class AuxRowWithPropsSnapshot
  {
    public Guid Id { get; init; }
    public Guid TableId { get; init; }
    public string? Clave { get; init; }
    public string? Nombre { get; init; }

    /// <summary>
    /// Mapa de propiedades solicitadas (propClave → propValor). Si la propiedad no existe para el row, su valor es null.
    /// </summary>
    public IReadOnlyDictionary<string, string?> Props { get; init; } = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
  }
}
