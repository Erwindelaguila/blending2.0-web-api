using AutoMapper;
using Function.Blending.Opt.Application.Abstractions.External;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

// Aliases
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start
{
  public sealed class StartLogEjecucionCommandHandler(
    ILogEjecucionRepository repo,
    IMapper mapper,
    IConfiguration cfg,
    IAppParamRepository appParams,
    IEstadoLogisticaCatalogService estados,
    ILogisticaModelStarter modelStarter
  ) : IRequestHandler<StartLogEjecucionCommand, Result<StartLogEjecucionResponse>>
  {
    public async Task<Result<StartLogEjecucionResponse>> Handle(StartLogEjecucionCommand request, CancellationToken ct)
    {
      // 1) Estado inicial (EnEjecucion) desde config
      var key = ConfigurationKeys.Catalog.LogisticExecutionStatus.EnEjecucion;
      var estadoStr = cfg[key];
      if (!Guid.TryParse(estadoStr, out var estadoInicialId))
        return Result<StartLogEjecucionResponse>.Fail($"Config inválida: '{key}' no es un GUID.");

      // 2) DTO -> VO
      VO.LogInpInfo? infoVo = mapper.Map<VO.LogInpInfo?>(request.Start.Info);
      VO.LogInpFiltro? filtroVo = mapper.Map<VO.LogInpFiltro?>(request.Start.Filtro);
      VO.LogInpOferta? ofertaVo = mapper.Map<VO.LogInpOferta?>(request.Start.Oferta);

      decimal timeout = await GetTiempoEsperaAsync(ct);

      if (filtroVo is not null) 
      {
        var division = await appParams.GetValueAsync(cfg[ConfigurationKeys.AppParam.Keys.Logistics.ValorDivision] ?? AppParamDefaults.Keys.LogisticaValorDivision, ct);
        filtroVo.Division = division ?? AppParamDefaults.Values.LogisticaValorDivision; 
        filtroVo.TiempoEspera = timeout;
      }

      // 3) Repo.Start      
      var entity = await repo.StartAsync(
        estadoInicialId,
        request.Start.Mensaje,
        request.CreadoPorId,
        infoVo,
        filtroVo,
        ofertaVo,
        ct
      );

      if (entity is null)
        return Result<StartLogEjecucionResponse>.Fail("No se pudo crear la ejecución.");

      // 4) Enriquecer Estado (VO) antes del mapping → igual que Calidad
      //    (Color vendrá solo si configuraste ExposeColor/ColorPropClave para Logística)
      var estadoSnapshot = await estados.GetByIdAsync(entity.EstadoId, ct);
      entity.Estado = estadoSnapshot;

      // ===== DISPARO SIN ESPERAR (fire-and-forget) =====
      _ = bool.TryParse(cfg[ConfigurationKeys.ExternalServices.EnableLogisticsModel], out var enableLogisticsModel);
      if (enableLogisticsModel)
      {
        var model = request.Model;
        model.EjecucionId = entity.Id;
        model.TiempoEspera = timeout;
        _ = modelStarter.StartAsync(model, CancellationToken.None); // NO await
      }

      // 5) Domain -> DTO
      var dto = mapper.Map<StartLogEjecucionResponse>(entity);
      return Result<StartLogEjecucionResponse>.Ok(dto);
    }

    private async Task<decimal> GetTiempoEsperaAsync(CancellationToken ct)
    {
      var timeoutFromDb = await appParams.GetValueAsync(cfg[ConfigurationKeys.AppParam.Keys.Logistics.TiempoEspera] ?? AppParamDefaults.Keys.LogisticaTiempoEspera, ct);
      _ = decimal.TryParse(timeoutFromDb, out var timeout);
      timeout = timeout > 0 ? timeout : AppParamDefaults.Values.LogisticaTiempoEspera;
      return timeout;
    }
  }
}
