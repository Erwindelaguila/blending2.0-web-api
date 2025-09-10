using Function.Blending.Opt.Domain.Entities;
using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    Guid creadoPorId,
    CalInpFiltro? filtro = null,
    IReadOnlyList<CalInpParametro>? parametros = null,
    CancellationToken ct = default
  );

  // === Actualización (Estado) ===
  Task<CalEjecucion?> SetEstadoAsync(Guid id, Guid nuevoEstadoId, Guid modificadoPorId, CancellationToken ct);

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
}
