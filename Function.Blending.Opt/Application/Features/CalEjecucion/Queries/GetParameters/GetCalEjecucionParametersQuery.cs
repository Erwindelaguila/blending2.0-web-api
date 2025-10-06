using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetParameters;

public sealed record GetCalEjecucionParametersQuery(Guid Id): IRequest<Result<IReadOnlyList<CalInpParametroDto>>>;
