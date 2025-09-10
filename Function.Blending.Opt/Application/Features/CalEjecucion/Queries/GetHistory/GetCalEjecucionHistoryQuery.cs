using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetHistory;

public sealed record GetCalEjecucionHistoryQuery(
  int Page,
  int PageSize,
  string? SortBy,
  string? SortDir,
  DateTime? CreadoDelUtc,
  DateTime? CreadoAlUtc,
  Guid? EstadoId,
  Guid? PlantaId,
  string? Codigo
) : IRequest<Result<PageResponse<CalEjecucionHistoryItemResponse>>>;
