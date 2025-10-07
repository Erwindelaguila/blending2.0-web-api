using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetInputById;

public sealed class GetCalEjecucionInputByIdQueryHandler(
    ICalEjecucionRepository repo,
    ICalEjecucionInputService inputService,
    IEstadoCalidadCatalogService estadosService,
    ITimeZoneService tz,
    ITimeZoneResolver tzResolver,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionInputByIdQuery, Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>> Handle(GetCalEjecucionInputByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>.Fail($"Execution '{request.Id}' not found.");

    // Disparar en paralelo (sin await todavía)
    var filtroTask = inputService.GetFilterByExecutionIdAsync(entity.Id, ct);
    var parametrosTask = inputService.GetParametersItemRmByExecutionIdAsync(entity.Id, ct);
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoCalidadSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    await Task.WhenAll(filtroTask, parametrosTask, estadoTask);

    // Leer resultados (los tasks ya están completados; await devuelve al instante)
    entity.Filtro = await filtroTask;
    entity.Parametros = await parametrosTask;
    entity.Estado ??= await estadoTask;

    var dto = mapper.Map<CalEjecucionResponse>(entity);
    var payload = DateConversionComposer.Wrap(dto, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>.Ok(payload);
  }
}