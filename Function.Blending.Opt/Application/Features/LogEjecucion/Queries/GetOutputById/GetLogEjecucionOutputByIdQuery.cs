
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetOutputById;

public sealed record GetLogEjecucionOutputByIdQuery(Guid Id) : IRequest<Result<LogEjecucionResponse>>;
