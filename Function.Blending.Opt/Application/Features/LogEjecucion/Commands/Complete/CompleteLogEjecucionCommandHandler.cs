using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Complete;

public sealed class CompleteLogEjecucionCommandHandler(
  ILogEjecucionRepository repo,
  IEstadoLogisticaCatalogService estados,
  IMapper mapper
) : IRequestHandler<CompleteLogEjecucionCommand, Result<LogEjecucionResponse>>
{
  public async Task<Result<LogEjecucionResponse>> Handle(CompleteLogEjecucionCommand request, CancellationToken ct)
  {
    // 1) Validar estado destino en catálogo (igual que Calidad)
    var estadoRef = await estados.GetByIdAsync(request.EstadoId, ct);
    if (estadoRef is null)
      return Result<LogEjecucionResponse>.Fail("Estado destino inválido para Logística.");

    // 2) Persistir cambio superficial (sin outputs por ahora)
    var entity = await repo.CompleteAsync(
      id: request.Id,
      estadoId: request.EstadoId,
      mensaje: request.Mensaje,
      modificadoPorId: request.ModificadoPorId,
      ct: ct);

    if (entity is null)
      return Result<LogEjecucionResponse>.NotFound("Execution not found.");

    // 3) Enriquecer VO Estado antes de mapear a DTO (patrón Calidad)
    entity.Estado = estadoRef;

    // 4) Mapear a respuesta estándar
    var dto = mapper.Map<LogEjecucionResponse>(entity);
    return Result<LogEjecucionResponse>.Ok(dto);
  }
}
