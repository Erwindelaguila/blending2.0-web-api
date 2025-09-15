using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetById;

public sealed class GetCalEjecucionByIdQueryHandler(
    ICalEjecucionRepository repo,
    IEstadoCalidadCatalogService estados,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionByIdQuery, Result<CalEjecucionResponse>>
{
  public async Task<Result<CalEjecucionResponse>> Handle(GetCalEjecucionByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<CalEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

    // Enriquecer Estado si falta (para asegurar objeto anidado consistente)
    if (entity.Estado is null)
      entity.Estado = await estados.GetByIdAsync(entity.EstadoId, ct);

    var dto = mapper.Map<CalEjecucionResponse>(entity);
    return Result<CalEjecucionResponse>.Ok(dto);
  }
}
