using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Entities;
using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;
using Microsoft.EntityFrameworkCore;


// ===== Aliases estandarizados =====
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;  // EF models
using D = Function.Blending.Opt.Domain.Entities;                    // Domain entities
using RM = Function.Blending.Opt.Domain.ReadModels;                 // Read models (CQRS)
using VO = Function.Blending.Opt.Domain.ValueObjects;               // Value objects

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class LogEjecucionRepository(
  BlendingDbContext db,
  IMapper mapper,
  IEstadoLogisticaCatalogService estados,
  IExecutionCodeGenerator codeGen,
  ICodeFormatProvider codeFormats
) : ILogEjecucionRepository
{
  public async Task<D.LogEjecucion?> StartAsync(
    Guid plantaId,
    Guid estadoInicialId,
    string? mensaje,
    Guid creadoPorId,
    LogInpInfo? info,
    LogInpFiltro? filtro,
    LogInpOferta? oferta,
    CancellationToken ct = default)
  {
    var id = Guid.NewGuid();

    var model = new E.LogEjecucion
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

    await using var tx = await db.Database.BeginTransactionAsync(ct);

    // 1) Insert ejecución con código temporal
    db.Set<E.LogEjecucion>().Add(model);
    await db.SaveChangesAsync(ct); // obtiene Secuencial

    // 2) Generar y fijar código final (único)
    var format = await codeFormats.GetLogisticExecutionFormatAsync(ct);
    var finalCode = codeGen.MakeFinal(format, model.Secuencial);

    var exists = await db.Set<E.LogEjecucion>()
                         .AsNoTracking()
                         .AnyAsync(e => e.Codigo == finalCode && e.Id != id, ct);
    if (exists)
      throw new InvalidOperationException($"Código duplicado inesperado: {finalCode}");

    model.Codigo = finalCode;
    await db.SaveChangesAsync(ct);

    // 3) Inputs profundos (Info/Filtro/Oferta + hijos) — mismo patrón que Calidad
    await LogisticaInputInserter.InsertAsync(db, mapper, model.Id, info, filtro, oferta, ct);

    // 4) Commit y map a Dominio
    await tx.CommitAsync(ct);
    return mapper.Map<D.LogEjecucion>(model);
  }

  public async Task<D.LogEjecucion?> CompleteAsync(
    Guid id,
    Guid estadoId,
    Guid modificadoPorId,
    string? mensaje,
    CancellationToken ct = default
  )
  {
    var model = await db.Set<E.LogEjecucion>()
                        .FirstOrDefaultAsync(x => x.Id == id, ct);
    if (model is null) return null;

    model.EstadoId = estadoId;
    model.Mensaje = mensaje;
    model.ModificadoEl = DateTime.UtcNow;
    model.ModificadoPorId = modificadoPorId;

    await db.SaveChangesAsync(ct);
    return mapper.Map<D.LogEjecucion>(model);
  }

  public async Task<LogEjecucion?> GetByIdAsync(Guid id, CancellationToken ct)
  {
    var model = await db.Set<E.LogEjecucion>()
                      .AsNoTracking()
                      .FirstOrDefaultAsync(x => x.Id == id, ct);

    return model is null ? null : mapper.Map<D.LogEjecucion>(model);
  }

  public async Task<(IReadOnlyList<LogEjecucionHistoryItemRm> Items, int Total)> GetHistoryAsync(
    int page, 
    int pageSize, 
    string? sortBy, 
    string? sortDir, 
    bool? confirmado, 
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

    var baseQuery = db.Set<E.LogEjecucion>().AsNoTracking();

    var filtered = LogEjecucionHistoryQuery.ApplyFilters(baseQuery, confirmado, creadoDelUtc, creadoAlUtc, estadoId, plantaId, codigo);

    var ordered = LogEjecucionHistoryQuery.ApplyOrdering(filtered, sortBy, sortDir);

    var total = await ordered.CountAsync(ct);
    var models = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    var items = mapper.Map<List<RM.LogEjecucionHistoryItemRm>>(models);

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

  public async Task<LogEjecucion?> SetEstadoAsync(Guid id, Guid nuevoEstadoId, Guid modificadoPorId, CancellationToken ct)
  {
    var set = db.Set<E.LogEjecucion>();
    var model = await set.FindAsync([id], ct);
    if (model is null) return null;

    model.EstadoId = nuevoEstadoId;
    model.ModificadoPorId = modificadoPorId;
    model.ModificadoEl = DateTime.UtcNow;

    await db.SaveChangesAsync(ct);
    return mapper.Map<D.LogEjecucion>(model);
  }

  public async Task<LogEjecucion?> SetConfirmadoAsync(Guid id, bool confirmado, Guid modificadoPorId, CancellationToken ct)
  {
    var set = db.Set<E.LogEjecucion>();
    var model = await set.FindAsync([id], ct);
    if (model is null) return null;

    model.Confirmado = confirmado;
    model.ModificadoPorId = modificadoPorId;
    model.ModificadoEl = DateTime.UtcNow;

    await db.SaveChangesAsync(ct);
    return mapper.Map<D.LogEjecucion>(model);
  }
}
