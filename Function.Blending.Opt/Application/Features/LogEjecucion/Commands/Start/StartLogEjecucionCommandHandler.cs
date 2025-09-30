using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

// Aliases
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start
{
  public sealed class StartLogEjecucionCommandHandler(
    ILogEjecucionRepository repo,
    IMapper mapper,
    IConfiguration cfg,
    IEstadoLogisticaCatalogService estados
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
      VO.LogInpInfo? infoVo = mapper.Map<VO.LogInpInfo?>(request.Info);
      VO.LogInpFiltro? filtroVo = mapper.Map<VO.LogInpFiltro?>(request.Filtro);
      VO.LogInpOferta? ofertaVo = mapper.Map<VO.LogInpOferta?>(request.Oferta);

      // 3) Repo.Start
      var entity = await repo.StartAsync(
        estadoInicialId,
        request.Mensaje,
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
      var estadoRef = await estados.GetByIdAsync(entity.EstadoId, ct);
      entity.Estado = estadoRef;

      // 5) Domain -> DTO
      var dto = mapper.Map<StartLogEjecucionResponse>(entity);
      return Result<StartLogEjecucionResponse>.Ok(dto);
    }
  }
}
