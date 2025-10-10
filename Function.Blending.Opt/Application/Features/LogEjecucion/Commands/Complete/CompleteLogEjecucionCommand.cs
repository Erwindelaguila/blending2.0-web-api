using Function.Blending.Opt.Application.Abstractions; // Result<T>
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Complete
{
  /// <summary>
  /// Superficial: cambia estado/mensaje y devuelve LogEjecucionResponse (igual que Calidad).
  /// </summary>
  public sealed record CompleteLogEjecucionCommand(
    Guid Id,
    Guid EstadoId,
    Guid ModificadoPorId,
    string? Mensaje
  ) : IRequest<Result<LogEjecucionResponse>>;
}
