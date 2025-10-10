using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Complete;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ChangeAccepted;

public sealed class ChangeAcceptedCalEjecucionCommandHandler(ICalEjecucionRepository repo) : IRequestHandler<ChangeAcceptedCalEjecucionCommand, Result<bool>>
{
  public async Task<Result<bool>> Handle(ChangeAcceptedCalEjecucionCommand request, CancellationToken ct)
  {
    var entity = await repo.SetAceptadoAsync(request.Id, request.Grupos, request.ModificadoPorId, ct);

    if (entity is null) return Result<bool>.NotFound("Execution not found.");

    return Result<bool>.Ok(true);
  }
}
