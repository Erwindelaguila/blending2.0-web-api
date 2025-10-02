using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetOutputById;

public sealed class GetLogEjecucionOutputByIdQueryHandler(
    ILogEjecucionRepository repo,
    ILogEjecucionOutputService outputService,
    IEstadoLogisticaCatalogService estadosService,
    IMapper mapper
  ) : IRequestHandler<GetLogEjecucionOutputByIdQuery, Result<LogEjecucionResponse>>
{
  public async Task<Result<LogEjecucionResponse>> Handle(GetLogEjecucionOutputByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<LogEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

    // Disparar en paralelo (sin await todavía)
    var contenedoresTask = outputService.GetDeepContenedoresByExcecutionAsync(entity.Id, ct);
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoLogisticaSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    await Task.WhenAll(contenedoresTask, estadoTask);

    // Leer resultados (los tasks ya están completados; await devuelve al instante)
    entity.Contenedores = await contenedoresTask;
    entity.Estado ??= await estadoTask;

    var dto = mapper.Map<LogEjecucionResponse>(entity);
    return Result<LogEjecucionResponse>.Ok(dto);
  }
}