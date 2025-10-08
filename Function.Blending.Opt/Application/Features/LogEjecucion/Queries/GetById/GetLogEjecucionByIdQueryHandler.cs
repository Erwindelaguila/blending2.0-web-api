using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetById;

public sealed class GetLogEjecucionByIdQueryHandler(
  ILogEjecucionRepository repo,
  ILogEjecucionInputService inputService,
  ILogEjecucionOutputService outputService,
  IEstadoLogisticaCatalogService estadosService,
  ITimeZoneService tz,
  ITimeZoneResolver tzResolver,
  IMapper mapper
) : IRequestHandler<GetLogEjecucionByIdQuery, Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>> Handle(GetLogEjecucionByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>.Fail($"Execution '{request.Id}' not found.");

    var estadoTask = entity.Estado is not null
      ? Task.FromResult((EstadoLogisticaSnapshot?)entity.Estado)
      : estadosService.GetByIdAsync(entity.EstadoId, ct);

    var tasks = new List<Task> { estadoTask };

    if (request.Expand.Contains("input")) tasks.Add(LoadInputAsync(entity, inputService, ct));
    if (request.Expand.Contains("output")) tasks.Add(LoadOutputAsync(entity, outputService, ct));

    await Task.WhenAll(tasks);
    entity.Estado ??= await estadoTask;

    var dto = mapper.Map<LogEjecucionResponse>(entity);

    var payload = DateConversionComposer.Wrap(dto, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>.Ok(payload);
  }

  static async Task LoadInputAsync(Domain.Entities.LogEjecucion entity, ILogEjecucionInputService inputService, CancellationToken ct)
  {
    var infoTask = inputService.GetInfoByExecutionIdAsync(entity.Id, ct);
    var filtroTask = inputService.GetFiltroByExecutionIdAsync(entity.Id, ct);
    var demandaTask = inputService.GetDemandaByExecutionIdAsync(entity.Id, ct);
    var ofertaTask = inputService.GetOfertaByExecutionIdAsync(entity.Id, ct);

    await Task.WhenAll(infoTask, ofertaTask, filtroTask);

    entity.Info = await infoTask;
    entity.Filtro = await filtroTask;
    entity.Demanda = await demandaTask;
    entity.Oferta = await ofertaTask;
  }

  static async Task LoadOutputAsync(Domain.Entities.LogEjecucion entity, ILogEjecucionOutputService outputService, CancellationToken ct)
  {
    var contenedoresTask = outputService.GetDeepContenedoresByExcecutionAsync(entity.Id, ct);
    _ = await Task.WhenAll(contenedoresTask);
    entity.Contenedores = await contenedoresTask;
  }
}
