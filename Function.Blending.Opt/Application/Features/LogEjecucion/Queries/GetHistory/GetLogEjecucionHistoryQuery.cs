using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetHistory;

public sealed record GetLogEjecucionHistoryQuery(
  int Page,
  int PageSize,
  string? SortBy,
  string? SortDir,
  bool? confirmado,
  DateTime? CreadoDelUtc,
  DateTime? CreadoAlUtc,
  Guid? EstadoId,
  string? Codigo
) : IRequest<Result<PageResponse<LogEjecucionHistoryItemResponse>>>;
