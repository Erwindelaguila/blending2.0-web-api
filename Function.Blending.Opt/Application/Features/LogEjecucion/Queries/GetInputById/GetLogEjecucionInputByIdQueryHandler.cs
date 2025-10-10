using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetInputById;

public sealed class GetLogEjecucionInputByIdQueryHandler(
    ILogEjecucionRepository repo,
    ILogEjecucionInputService inputService,
    IEstadoLogisticaCatalogService estadosService,
    ITimeZoneService tz,
    ITimeZoneResolver tzResolver,
    IMapper mapper
  ) : IRequestHandler<GetLogEjecucionInputByIdQuery, Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>> Handle(GetLogEjecucionInputByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>.Fail($"Execution '{request.Id}' not found.");

    // Disparar en paralelo (sin await todavía)
    var infoTask = inputService.GetInfoByExecutionIdAsync(entity.Id, ct);
    var ofertaTask = inputService.GetOfertaByExecutionIdAsync(entity.Id, ct);
    var filtroTask = inputService.GetFiltroByExecutionIdAsync(entity.Id, ct);
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoLogisticaSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    await Task.WhenAll(infoTask, ofertaTask, filtroTask, estadoTask);

    // Leer resultados (los tasks ya están completados; await devuelve al instante)
    entity.Info = await infoTask;
    entity.Oferta = await ofertaTask;
    entity.Filtro = await filtroTask;
    entity.Estado ??= await estadoTask;

    var dto = mapper.Map<LogEjecucionResponse>(entity);

    var payload = DateConversionComposer.Wrap(dto, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>.Ok(payload);
  }
}