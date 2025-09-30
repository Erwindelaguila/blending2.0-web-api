using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Function.Blending.Opt.Application.Abstractions.External;
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
    IEstadoCalidadCatalogService estadosService,
    IConfiguration cfg,
    IMapper mapper,
    ICalidadModelStarter modelStarter
  ) : IRequestHandler<StartCalEjecucionCommand, Result<StartCalEjecucionResponse>>
{
  public async Task<Result<StartCalEjecucionResponse>> Handle(StartCalEjecucionCommand request, CancellationToken ct)
  {
    var rawEstado = cfg[ConfigurationKeys.Catalog.QualityExecutionStatus.EnEjecucion];
    if (!Guid.TryParse(rawEstado, out var estadoInicialId))
      return Result<StartCalEjecucionResponse>.Fail("Estado inicial (EnEjecucion) no configurado correctamente.");

    CalInpFiltro? filtroVo = null;
    if (request.Start.Filtro is not null)
      filtroVo = mapper.Map<CalInpFiltro>(request.Start.Filtro);

    IReadOnlyList<CalInpParametro>? parametrosVo = null;
    if (request.Start.Parametros is { Count: > 0 })
    {
      var normalizedDtos = request.Start.Parametros
        .GroupBy(p => new { p.CalidadId, p.ParametroId })
        .Select(g => g.Last())
        .ToList();

      parametrosVo = mapper.Map<IReadOnlyList<CalInpParametro>>(normalizedDtos);
    }

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

    entity.Estado = await estadosService.GetByIdAsync(entity.EstadoId.Value, ct);

    // ===== DISPARO SIN ESPERAR (fire-and-forget) =====
    _ = bool.TryParse(cfg[ConfigurationKeys.ExternalServices.EnableQualityModel], out var enableQualityModel);
    if (enableQualityModel)
    {
      var model = request.Model;
      model.EjecucionId = entity.Id;
      _ = modelStarter.StartAsync(model, CancellationToken.None); // NO await
    }

    var dto = mapper.Map<StartCalEjecucionResponse>(entity);
    return Result<StartCalEjecucionResponse>.Ok(dto);
  }
}
