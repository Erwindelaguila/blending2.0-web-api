using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Complete;

public sealed class CompleteCalEjecucionCommandHandler(
  ICalEjecucionRepository repo,
  IEstadoCalidadCatalogService estados,
  IMapper mapper
) : IRequestHandler<CompleteCalEjecucionCommand, Result<CalEjecucionResponse>>
{
  public async Task<Result<CalEjecucionResponse>> Handle(CompleteCalEjecucionCommand request, CancellationToken ct)
  {
    // 1) Validar estado destino existe en catálogo
    var estadoRef = await estados.GetByIdAsync(request.EstadoId, ct);
    if (estadoRef is null)
      return Result<CalEjecucionResponse>.Fail("Estado destino inválido para Calidad.");

    // 2) Mapear DTO → VO (pueden venir null)
    IReadOnlyList<CalOutResumen>? voResumenes = request.Resumenes is { Count: > 0 } ? mapper.Map<List<CalOutResumen>>(request.Resumenes) : null;

    IReadOnlyList<CalOutDetalle>? voDetalles = request.Detalles is { Count: > 0 } ? mapper.Map<List<CalOutDetalle>>(request.Detalles) : null;

    // 3) Persistir (transacción única; inserta outputs una vez y cambia estado)
    var entity = await repo.CompleteAsync(
      request.Id,
      request.EstadoId,
      request.Mensaje,
      request.ModificadoPorId,
      voResumenes,
      voDetalles,
      ct);

    if (entity is null)
      return Result<CalEjecucionResponse>.NotFound("Execution not found.");

    entity.Estado = estadoRef;

    var dto = mapper.Map<CalEjecucionResponse>(entity);
    return Result<CalEjecucionResponse>.Ok(dto);
  }
}
