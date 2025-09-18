using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;

public sealed class StartCalEjecucionCommandHandler(
    ICalEjecucionRepository repo,
    IEstadoCalidadCatalogService estados,
    IConfiguration cfg,
    IMapper mapper
  ) : IRequestHandler<StartCalEjecucionCommand, Result<StartCalEjecucionResponse>>
{
  public async Task<Result<StartCalEjecucionResponse>> Handle(StartCalEjecucionCommand request, CancellationToken ct)
  {
    // Estado inicial: EnEjecucion (desde config)
    var raw = cfg[ConfigurationKeys.Catalog.QualityExecutionStatus.EnEjecucion];
    if (!Guid.TryParse(raw, out var estadoInicialId))
      return Result<StartCalEjecucionResponse>.Fail("Estado inicial (EnEjecucion) no configurado correctamente.");

    // DTO -> VO (opcional)
    CalInpFiltro? filtroVo = null;
    if (request.Start.Filtro is not null)
      filtroVo = mapper.Map<CalInpFiltro>(request.Start.Filtro);

    IReadOnlyList<CalInpParametro>? parametrosVo = null;
    if (request.Start.Parametros is { Count: > 0 })
    {
      // Dedupe por (CalidadId, ParametroId): último gana
      var normalizedDtos = request.Start.Parametros
        .GroupBy(p => new { p.CalidadId, p.ParametroId })
        .Select(g => g.Last())
        .ToList();

      parametrosVo = mapper.Map<IReadOnlyList<CalInpParametro>>(normalizedDtos);
    }

    // Crear ejecución (una transacción; StartAsync acepta VO opcionales; ct al final)
    var entity = await repo.StartAsync(
      request.Start.PlantaId,
      estadoInicialId,
      request.Start.Mensaje,
      request.CreadoPorId,
      filtroVo,
      parametrosVo,
      ct
    );

    if (entity is null)
      return Result<StartCalEjecucionResponse>.Fail("No se pudo crear la ejecución.");

    // TODO: Aquí consumir el servicio externo API de Calidad
    var model = request.Model;
    model.EjecucionId = entity.Id;

    // Enriquecer Estado (objeto anidado en la respuesta)
    entity.Estado = await estados.GetByIdAsync(entity.EstadoId.Value, ct);

    var dto = mapper.Map<StartCalEjecucionResponse>(entity);
    return Result<StartCalEjecucionResponse>.Ok(dto);
  }
}
