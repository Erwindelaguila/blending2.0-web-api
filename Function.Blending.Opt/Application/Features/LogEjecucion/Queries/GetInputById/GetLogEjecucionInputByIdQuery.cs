using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetInputById;

public sealed record GetLogEjecucionInputByIdQuery(
  Guid Id,
  bool ConvertDates,
  string? TzId
) : IRequest<Result<WithMeta<LogEjecucionResponse, DateConversionMeta>>>;