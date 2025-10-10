using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

/// <summary>
/// Foto de una fila de un catálogo Aux*, con sus valores de propiedades (clave->valor).
/// </summary>
public sealed class AuxRowSnapshot
{
  public Guid Id { get; init; }
  public Guid TableId { get; init; }
  public string Clave { get; init; } = default!;
  public string Nombre { get; init; } = default!;
  public IReadOnlyDictionary<string, string> Props { get; init; } = new Dictionary<string, string>();
}
