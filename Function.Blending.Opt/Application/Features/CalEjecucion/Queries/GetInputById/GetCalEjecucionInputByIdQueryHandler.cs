using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetInputById;

public sealed class GetCalEjecucionInputByIdQueryHandler(
    ICalEjecucionRepository repo,
    ICalEjecucionInputService inputService,
    IEstadoCalidadCatalogService estadosService,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionInputByIdQuery, Result<CalEjecucionResponse>>
{
  public async Task<Result<CalEjecucionResponse>> Handle(GetCalEjecucionInputByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<CalEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

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
    return Result<CalEjecucionResponse>.Ok(dto);
  }
}