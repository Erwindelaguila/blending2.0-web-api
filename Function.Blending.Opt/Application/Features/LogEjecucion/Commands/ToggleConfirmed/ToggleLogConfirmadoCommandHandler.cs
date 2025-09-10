
using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.ToggleState;

public sealed class ToggleLogConfirmadoCommandHandler(
  ILogEjecucionRepository repo,
  IEstadoLogisticaCatalogService estados,
  IConfiguration cfg,
  IMapper mapper
) : IRequestHandler<ToggleLogConfirmadoCommand, Result<LogEjecucionResponse>>
{
  public async Task<Result<LogEjecucionResponse>> Handle(ToggleLogConfirmadoCommand request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<LogEjecucionResponse>.NotFound("Execution not found.");

    var confirmado = entity.Confirmado.HasValue && entity.Confirmado.Value;

    // Toggle 
    var updated = await repo.SetConfirmadoAsync(request.Id, !confirmado, request.ModificadoPorId, ct);
    if (updated is null)
      return Result<LogEjecucionResponse>.NotFound("Execution not found.");

    // Enriquecer VO y mapear (igual que tenías)
    updated.Estado = await estados.GetByIdAsync(entity.EstadoId, ct);
    var dto = mapper.Map<LogEjecucionResponse>(updated);

    return Result<LogEjecucionResponse>.Ok(dto);
  }
}
