using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetById;

public sealed record GetLogEjecucionByIdQuery(Guid Id, HashSet<string> expand) : IRequest<Result<LogEjecucionResponse>>;