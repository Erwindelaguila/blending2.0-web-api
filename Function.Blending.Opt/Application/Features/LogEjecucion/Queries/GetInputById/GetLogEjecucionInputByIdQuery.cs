using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetInputById;

public sealed record GetLogEjecucionInputByIdQuery(Guid Id) : IRequest<Result<LogEjecucionResponse>>;