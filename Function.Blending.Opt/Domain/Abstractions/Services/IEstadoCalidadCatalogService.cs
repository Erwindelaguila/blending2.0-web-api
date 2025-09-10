using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

/// <summary>
/// Servicio de catálogo para los <b>Estados de Calidad</b> (filas de AuxRow pertenecientes a la AuxTable configurada).
/// </summary>
public interface IEstadoCalidadCatalogService
{
  /// <summary>Obtiene una referencia de estado por Id (valida TableId si está configurado).</summary>
  Task<EstadoCalidadRef?> GetByIdAsync(Guid id, CancellationToken ct);

  /// <summary>Obtiene referencias de estado para varios Ids (valida TableId si está configurado).</summary>
  Task<IDictionary<Guid, EstadoCalidadRef>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
}
