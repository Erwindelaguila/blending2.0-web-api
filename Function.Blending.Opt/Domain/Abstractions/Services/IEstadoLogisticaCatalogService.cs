using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

/// <summary>
/// Servicio de catálogo para los <b>Estados de Logística</b> (filas de AuxRow pertenecientes a la AuxTable configurada).
/// </summary>
public interface IEstadoLogisticaCatalogService
{
  /// <summary>Obtiene una referencia de estado por Id (valida TableId si está configurado).</summary>
  Task<EstadoLogisticaSnapshot?> GetByIdAsync(Guid id, CancellationToken ct);

  /// <summary>Obtiene referencias de estado para varios Ids (valida TableId si está configurado).</summary>
  Task<IDictionary<Guid, EstadoLogisticaSnapshot>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
}
