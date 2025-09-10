using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetById;

public sealed class GetLogEjecucionByIdQueryHandler(
  ILogEjecucionRepository repo,
  IEstadoLogisticaCatalogService estados,
  IMapper mapper
) : IRequestHandler<GetLogEjecucionByIdQuery, Result<LogEjecucionResponse>>
{
  public async Task<Result<LogEjecucionResponse>> Handle(GetLogEjecucionByIdQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<LogEjecucionResponse>.Fail($"Execution '{request.Id}' not found.");

    if (entity.Estado is null)
      entity.Estado = await estados.GetByIdAsync(entity.EstadoId, ct);

    var dto = mapper.Map<LogEjecucionResponse>(entity);
    return Result<LogEjecucionResponse>.Ok(dto);
  }
}
