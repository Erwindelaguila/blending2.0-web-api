using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ToggleState;

public sealed class ToggleCalEstadoCommandHandler(
  ICalEjecucionRepository repo,
  IEstadoCalidadCatalogService estados,
  IConfiguration cfg,
  IMapper mapper
) : IRequestHandler<ToggleCalEstadoCommand, Result<CalEjecucionResponse>>
{
  public async Task<Result<CalEjecucionResponse>> Handle(ToggleCalEstadoCommand request, CancellationToken ct)
  {
    // GUIDs desde config (mantengo tu lectura de catálogo por claves)
    var rawProc = cfg[ConfigurationKeys.Catalog.QualityExecutionStatus.Procesado];
    var rawAcept = cfg[ConfigurationKeys.Catalog.QualityExecutionStatus.Aceptado];

    if (!Guid.TryParse(rawProc, out var procesadoId) || !Guid.TryParse(rawAcept, out var aceptadoId))
      return Result<CalEjecucionResponse>.Fail("Estados permitidos (Procesado/Aceptado) no configurados correctamente.");

    // Cargar ejecución (igual que tenías)
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<CalEjecucionResponse>.NotFound("Execution not found.");

    // Validar estado actual (igual que tenías)
    var current = entity.EstadoId;
    var esProcesadoOAceptado = current == procesadoId || current == aceptadoId;
    if (!esProcesadoOAceptado)
      return Result<CalEjecucionResponse>.Fail("La ejecución no está en un estado intercambiable (Procesado/Aceptado).");

    // Toggle (igual que tenías)
    var target = (current == procesadoId) ? aceptadoId : procesadoId;

    // Persistir nuevo estado (ahora con auditoría ModificadoPorId)
    var updated = await repo.SetEstadoAsync(request.Id, target, request.ModificadoPorId, ct);
    if (updated is null)
      return Result<CalEjecucionResponse>.NotFound("Execution not found.");

    // Enriquecer VO y mapear (igual que tenías)
    updated.Estado = await estados.GetByIdAsync(target, ct);
    var dto = mapper.Map<CalEjecucionResponse>(updated);

    return Result<CalEjecucionResponse>.Ok(dto);
  }
}
