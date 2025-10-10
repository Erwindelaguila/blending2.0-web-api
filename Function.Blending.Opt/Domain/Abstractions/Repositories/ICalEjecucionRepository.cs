using Function.Blending.Opt.Domain.Entities;
using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ICalEjecucionRepository
{
  // === Lectura existentes ===
  Task<CalEjecucion?> GetByIdAsync(Guid id, CancellationToken ct);

  Task<(IReadOnlyList<CalEjecucionHistoryItemRm> Items, int Total)> GetHistoryAsync(
    int page,
    int pageSize,
    string? sortBy,
    string? sortDir,
    DateTime? creadoDelUtc,
    DateTime? creadoAlUtc,
    Guid? estadoId,
    Guid? plantaId,
    string? codigo,
    CancellationToken ct
  );

  // === Creación (Start) con auditoría ===
  Task<CalEjecucion?> StartAsync(
    Guid plantaId,
    Guid estadoInicialId,
    string? mensaje,
    string? nombreArchivo,
    string? urlArchivo,
    Guid creadoPorId,
    CalInpFiltro? filtro = null,
    IReadOnlyList<CalInpParametro>? parametros = null,
    CancellationToken ct = default
  );

  // === Actualización (Complete) ===
  Task<CalEjecucion?> CompleteAsync(
    Guid id,
    Guid estadoDestinoId,
    string? mensaje,
    Guid modificadoPorId,
    IReadOnlyList<CalOutResumen>? resumenes = null,
    IReadOnlyList<CalOutDetalle>? detalles = null,
    CancellationToken ct = default
  );

  // === Actualización (Cambiar Aceptado, en Grupos) ===
  Task<bool?> SetAceptadoAsync(Guid id, IReadOnlyList<Guid>? Grupos, Guid modificadoPorId, CancellationToken ct);

}
