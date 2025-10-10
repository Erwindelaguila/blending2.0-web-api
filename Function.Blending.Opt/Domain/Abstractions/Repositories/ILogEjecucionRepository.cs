using Function.Blending.Opt.Domain.Entities;
using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories
{
  public interface ILogEjecucionRepository
  {
    Task<LogEjecucion?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<(IReadOnlyList<LogEjecucionHistoryItemRm> Items, int Total)> GetHistoryAsync(
      int page,
      int pageSize,
      string? sortBy,
      string? sortDir,
      bool? confirmado,
      DateTime? creadoDelUtc,
      DateTime? creadoAlUtc,
      Guid? estadoId,
      string? codigo,
      string? contrato,
      CancellationToken ct
    );

    Task<LogEjecucion?> StartAsync(
      Guid estadoInicialId,
      string? mensaje,
      string? nombreArchivo,
      string? urlArchivo,
      Guid creadoPorId,
      LogInpInfo? info,
      LogInpFiltro? filtro,
      LogInpDemanda? demanda,
      IReadOnlyList<LogInpOferta>? oferta,
      CancellationToken ct = default);
    
    Task<LogEjecucion?> SetEstadoAsync(Guid id, Guid nuevoEstadoId, Guid modificadoPorId, CancellationToken ct);

    Task<LogEjecucion?> SetConfirmadoAsync(Guid id, bool confirmado, Guid modificadoPorId, CancellationToken ct);

    Task<LogEjecucion?> CompleteAsync(
      Guid id,
      Guid estadoId,
      Guid modificadoPorId,
      string? mensaje,
      CancellationToken ct = default
    );
  }
}
