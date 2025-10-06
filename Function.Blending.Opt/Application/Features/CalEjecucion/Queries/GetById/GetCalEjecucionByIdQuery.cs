using System;
using MediatR;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetById;

public sealed record GetCalEjecucionByIdQuery(Guid Id, HashSet<string> expand) : IRequest<Result<CalEjecucionResponse>>;
