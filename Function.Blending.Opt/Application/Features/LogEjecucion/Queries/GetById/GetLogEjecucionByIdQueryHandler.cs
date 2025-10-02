using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
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
  IMapper mapper
) : IRequestHandler<GetLogEjecucionByIdQuery, Result<LogEjecucionResponse>>
{
  public async Task<Result<LogEjecucionResponse>> Handle(GetLogEjecucionByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<LogEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

    // Dispara la carga de Estado (evita IO si ya viene cargado)
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoLogisticaSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    // Fan-out paralelo según expand
    var tasks = new List<Task> { estadoTask };

    if (request.expand.Contains("input")) tasks.Add(LoadInputAsync(entity, inputService, ct));

    if (request.expand.Contains("output")) tasks.Add(LoadOutputAsync(entity, outputService, ct));

    await Task.WhenAll(tasks); // si algo falla, lo manejará el middleware

    entity.Estado ??= await estadoTask; // completar si faltaba

    var dto = mapper.Map<LogEjecucionResponse>(entity);
    return Result<LogEjecucionResponse>.Ok(dto);
  }

  static async Task LoadInputAsync(Domain.Entities.LogEjecucion entity, ILogEjecucionInputService inputService, CancellationToken ct)
  {
    var infoTask = inputService.GetInfoByExecutionIdAsync(entity.Id, ct);
    var ofertaTask = inputService.GetOfertaByExecutionIdAsync(entity.Id, ct);
    var filtroTask = inputService.GetFiltroByExecutionIdAsync(entity.Id, ct);

    await Task.WhenAll(infoTask, ofertaTask, filtroTask);

    entity.Info = await infoTask;
    entity.Oferta = await ofertaTask;
    entity.Filtro = await filtroTask;
  }

  static async Task LoadOutputAsync(Domain.Entities.LogEjecucion entity, ILogEjecucionOutputService outputService, CancellationToken ct)
  {
    var contenedoresTask = outputService.GetDeepContenedoresByExcecutionAsync(entity.Id, ct);

    await Task.WhenAll(contenedoresTask);

    entity.Contenedores = await contenedoresTask;
  }
}
