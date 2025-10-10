using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

// Record con *primary constructor*; 1:N con UQ(EjecucionId, CalidadId, ParametroId)
public sealed record CalInpParametroDto(
  Guid CalidadId,
  Guid ParametroId,
  decimal Valor
);
