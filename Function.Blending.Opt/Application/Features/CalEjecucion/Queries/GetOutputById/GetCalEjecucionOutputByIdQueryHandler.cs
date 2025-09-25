using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetOutputById;

public sealed class GetCalEjecucionOutputByIdQueryHandler(
    ICalEjecucionRepository repo,
    ICalEjecucionOutService outputService,
    IEstadoCalidadCatalogService estadosService,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionOutputByIdQuery, Result<CalEjecucionResponse>>
{
  public async Task<Result<CalEjecucionResponse>> Handle(GetCalEjecucionOutputByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<CalEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

    // Disparar en paralelo
    var summariesTask = outputService.GetDeepSummariesByExcecutionAsync(entity.Id, ct);
    var detailsTask = outputService.GetDeepDetailsByExcecutionAsync(entity.Id, ct);
    var estadoTask = entity.Estado is not null ? Task.FromResult((EstadoCalidadSnapshot?)entity.Estado) : estadosService.GetByIdAsync(entity.EstadoId, ct);

    await Task.WhenAll(summariesTask, detailsTask, estadoTask);

    // Asignar resultados
    entity.Resumenes = await summariesTask;
    entity.Detalles = await detailsTask;
    entity.Estado ??= await estadoTask;

    var dto = mapper.Map<CalEjecucionResponse>(entity);
    return Result<CalEjecucionResponse>.Ok(dto);
  }
}
