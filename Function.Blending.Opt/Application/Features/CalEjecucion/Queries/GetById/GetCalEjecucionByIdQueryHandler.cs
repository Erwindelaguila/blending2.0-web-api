using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetById;

public sealed class GetCalEjecucionByIdQueryHandler(
    ICalEjecucionRepository repo,
    ICalEjecucionInputService inputService,
    ICalEjecucionOutputService outputService,
    IEstadoCalidadCatalogService estadosService,
    ITimeZoneService tz,
    ITimeZoneResolver tzResolver,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionByIdQuery, Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>> Handle(GetCalEjecucionByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>.Fail($"Execution '{request.Id}' not found.");

    // Dispara la carga de Estado (evita IO si ya viene cargado)
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoCalidadSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    // Fan-out paralelo según expand
    var tasks = new List<Task> { estadoTask };

    if (request.Expand.Contains("input")) tasks.Add(LoadInputAsync(entity, inputService, ct));

    if (request.Expand.Contains("output")) tasks.Add(LoadOutputAsync(entity, outputService, ct));

    await Task.WhenAll(tasks); // si algo falla, lo manejará el middleware

    entity.Estado ??= await estadoTask; // completar si faltaba

    var dto = mapper.Map<CalEjecucionResponse>(entity);

    var payload = DateConversionComposer.Wrap(dto, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>.Ok(payload);
  }

  static async Task LoadInputAsync(Domain.Entities.CalEjecucion entity, ICalEjecucionInputService inputService, CancellationToken ct)
  {
    var filtroTask = inputService.GetFilterByExecutionIdAsync(entity.Id, ct);
    var parametrosTask = inputService.GetParametersItemRmByExecutionIdAsync(entity.Id, ct);

    await Task.WhenAll(filtroTask, parametrosTask);

    entity.Filtro = await filtroTask;
    entity.Parametros = await parametrosTask;
  }

  static async Task LoadOutputAsync(Domain.Entities.CalEjecucion entity, ICalEjecucionOutputService outputService, CancellationToken ct)
  {
    var summariesTask = outputService.GetDeepSummariesByExcecutionAsync(entity.Id, ct);
    var detailsTask = outputService.GetDeepDetailsByExcecutionAsync(entity.Id, ct);

    await Task.WhenAll(summariesTask, detailsTask);

    entity.Resumenes = await summariesTask;
    entity.Detalles = await detailsTask;
  }
}
