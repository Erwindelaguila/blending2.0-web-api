using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetInputById;

public sealed record GetCalEjecucionInputByIdQuery(Guid Id): IRequest<Result<CalEjecucionResponse>>;
