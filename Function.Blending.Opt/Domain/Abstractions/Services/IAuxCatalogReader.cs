using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

/// <summary>Lector genérico sobre tablas Aux* (Table/Row/Prop/Value).</summary>
public interface IAuxCatalogReader
{
  /// <summary>Carga encabezado de fila: Id, TableId, Nombre.</summary>
  Task<AuxRowSnapshot?> GetRowHeaderAsync(Guid rowId, CancellationToken ct);

  /// <summary>Devuelve nombres por Id para un conjunto de filas.</summary>
  Task<Dictionary<Guid, string>> GetRowNamesAsync(IEnumerable<Guid> rowIds, CancellationToken ct);

  /// <summary>Devuelve el valor (string) de una propiedad (por clave) para una fila dada.</summary>
  Task<string?> GetRowPropValueAsync(Guid rowId, string propClave, CancellationToken ct);

  /// <summary>Devuelve valores (string) por RowId para una propiedad (por clave) para múltiples filas.</summary>
  Task<Dictionary<Guid, string>> GetRowPropValuesAsync(IEnumerable<Guid> rowIds, string propClave, CancellationToken ct);

  Task<AuxRowWithPropsSnapshot?> GetRowWithPropsAsync(Guid rowId, IEnumerable<string> propClaves, CancellationToken ct);

  Task<IReadOnlyList<AuxRowWithPropsSnapshot>> GetRowsWithPropsAsync(IEnumerable<Guid> rowIds, IEnumerable<string> propClaves, CancellationToken ct);
}
