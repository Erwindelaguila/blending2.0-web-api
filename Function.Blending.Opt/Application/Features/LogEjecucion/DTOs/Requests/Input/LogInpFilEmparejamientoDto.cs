using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

public sealed record LogInpFilEmparejamientoDto(
  string Grupo,
  Guid ParametroId,
  decimal Valor
);
