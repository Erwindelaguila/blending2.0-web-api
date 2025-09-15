using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ChangeAccepted;

public sealed record ChangeAcceptedCalEjecucionCommand(Guid Id, IReadOnlyList<Guid>? Grupos, Guid ModificadoPorId) : IRequest<Result<bool>>;
