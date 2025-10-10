using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetOutputById;

public sealed record GetCalEjecucionOutputByIdQuery(
  Guid Id,
  bool ConvertDates,
  string? TzId
) : IRequest<Result<WithMeta<CalEjecucionResponse, DateConversionMeta>>>;

