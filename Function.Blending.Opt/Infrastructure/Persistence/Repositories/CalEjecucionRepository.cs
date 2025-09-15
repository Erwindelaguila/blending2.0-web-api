using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;                   
using Microsoft.EntityFrameworkCore;

// ===== Alias estandarizados =====
using D = Function.Blending.Opt.Domain.Entities;                    // Domain entities
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;  // EF models
using RM = Function.Blending.Opt.Domain.ReadModels;                 // Read models (CQRS)
using VO = Function.Blending.Opt.Domain.ValueObjects;               // Value objects

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class CalEjecucionRepository(
  BlendingDbContext db,
  IMapper mapper,
  IEstadoCalidadCatalogService estados,
  IExecutionCodeGenerator codeGen,
  ICodeFormatProvider codeFormats
) : ICalEjecucionRepository
{
  // ============================
  // GetById
  // ============================
  public async Task<D.CalEjecucion?> GetByIdAsync(Guid id, CancellationToken ct)
  {
    var model = await db.Set<E.CalEjecucion>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id, ct);

    return model is null ? null : mapper.Map<D.CalEjecucion>(model);
  }

  // ============================
  // History (proyección ligera)
  // ============================
  public async Task<(IReadOnlyList<RM.CalEjecucionHistoryItemRm> Items, int Total)> GetHistoryAsync(
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
  )
  {
    page = page < 1 ? 1 : page;
    pageSize = pageSize < 1 ? 1 : pageSize > 200 ? 200 : pageSize;

    var baseQuery = db.Set<E.CalEjecucion>().AsNoTracking();

    var filtered = CalEjecucionHistoryQuery.ApplyFilters(baseQuery,
      creadoDelUtc, creadoAlUtc, estadoId, plantaId, codigo);

    var ordered = CalEjecucionHistoryQuery.ApplyOrdering(filtered, sortBy, sortDir);

    var total = await ordered.CountAsync(ct);
    var models = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    var items = mapper.Map<List<RM.CalEjecucionHistoryItemRm>>(models);

    // Enriquecer nombres de estado (batch)
    var estadoIds = items.Select(i => i.EstadoId).Distinct().ToArray();
    if (estadoIds.Length > 0)
    {
      var dict = await estados.GetByIdsAsync(estadoIds, ct);
      foreach (var it in items)
        if (dict.TryGetValue(it.EstadoId, out var est) && est is not null)
          it.EstadoNombre = est.Nombre;
    }

    return (items, total);
  }

  // ============================
  // Start (con Input opcional)
  // ============================
  public async Task<D.CalEjecucion?> StartAsync(
    Guid plantaId,
    Guid estadoInicialId,
    string? mensaje,
    Guid creadoPorId,
    VO.CalInpFiltro? filtro = null,
    IReadOnlyList<VO.CalInpParametro>? parametros = null,
    CancellationToken ct = default
  )
  {
    var id = Guid.NewGuid();

    var model = new E.CalEjecucion
    {
      Id = id,
      PlantaId = plantaId,
      EstadoId = estadoInicialId,
      Codigo = codeGen.MakeTemp(),
      Mensaje = mensaje,
      CreadoEl = DateTime.UtcNow,
      CreadoPorId = creadoPorId,
      ModificadoEl = null,
      ModificadoPorId = null
    };

    using var tx = await db.Database.BeginTransactionAsync(ct);

    db.Set<E.CalEjecucion>().Add(model);
    await db.SaveChangesAsync(ct); // carga Secuencial

    var format = await codeFormats.GetQualityExecutionFormatAsync(ct);
    var finalCode = codeGen.MakeFinal(format, model.Secuencial);

    var exists = await db.Set<E.CalEjecucion>()
                         .AsNoTracking()
                         .AnyAsync(e => e.Codigo == finalCode && e.Id != id, ct);
    if (exists)
      throw new InvalidOperationException($"Código duplicado inesperado: {finalCode}");

    model.Codigo = finalCode;
    await db.SaveChangesAsync(ct);

    // Input opcional
    await CalidadInputInserter.InsertAsync(db, mapper, model.Id, filtro, parametros, ct);

    await tx.CommitAsync(ct);
    return mapper.Map<D.CalEjecucion>(model);
  }

  // ============================
  // Complete (Output + estado)
  // ============================
  public async Task<D.CalEjecucion?> CompleteAsync(
    Guid id,
    Guid estadoDestinoId,
    string? mensaje,
    Guid modificadoPorId,
    IReadOnlyList<VO.CalOutResumen>? resumenes = null,
    IReadOnlyList<VO.CalOutDetalle>? detalles = null,
    CancellationToken ct = default
  )
  {
    var set = db.Set<E.CalEjecucion>();
    var model = await set.FindAsync([id], ct);
    if (model is null) return null;

    using var tx = await db.Database.BeginTransactionAsync(ct);

    var outRes = await db.Set<E.CalOutResumen>().AsNoTracking().Where(x => x.EjecucionId == id).ToListAsync(ct);
    var outDet = await db.Set<E.CalOutDetalle>().AsNoTracking().Where(x => x.EjecucionId == id).ToListAsync(ct);

    // ¿Ya existen outputs? (idempotencia)
    var outResExists = await db.Set<E.CalOutResumen>().AsNoTracking().AnyAsync(x => x.EjecucionId == id, ct);
    var outDetExists = await db.Set<E.CalOutDetalle>().AsNoTracking().AnyAsync(x => x.EjecucionId == id, ct);

    if (!outResExists && resumenes is { Count: > 0 })
      await CalidadOutputInserter.InsertResumenesAsync(db, mapper, id, modificadoPorId, resumenes, ct);

    if (!outDetExists && detalles is { Count: > 0 })
      await CalidadOutputInserter.InsertDetallesAsync(db, mapper, id, modificadoPorId, detalles, ct);

    // Cambiar estado + auditoría
    model.EstadoId = estadoDestinoId;
    model.Mensaje = mensaje;
    model.ModificadoPorId = modificadoPorId;
    model.ModificadoEl = DateTime.UtcNow;

    await db.SaveChangesAsync(ct);
    await tx.CommitAsync(ct);

    return mapper.Map<D.CalEjecucion>(model);
  }

  // ============================
  // SetAceptado
  // ============================
  public async Task<bool?> SetAceptadoAsync(Guid id, IReadOnlyList<Guid>? grupos, Guid modificadoPorId, CancellationToken ct)
  {
    var set = db.Set<E.CalEjecucion>();
    var model = await set.FindAsync([id], ct);
    if (model is null) return null;

    if (grupos is null || grupos.Count == 0) return false;

    using var tx = await db.Database.BeginTransactionAsync(ct);

    (List<E.CalOutResumen> resumenesYaNoAceptados, List<E.CalOutResumen> nuevosResumenesAceptados) = await CalidadOutputUpdater.UpdaterResumenesAsync(db, id, grupos, modificadoPorId, ct);
    await CalidadOutputUpdater.UpdaterDetallesAsync(db, id, modificadoPorId, resumenesYaNoAceptados, nuevosResumenesAceptados, ct);
    await CalidadOutputUpdater.SaveChangesAsync(db, modificadoPorId, model, ct);

    await tx.CommitAsync(ct);

    return true;
  }
}
