using AutoMapper;
using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetHistory;

public sealed class GetLogEjecucionHistoryQueryHandler(
  ILogEjecucionRepository repo,
  ITimeZoneService tz,
  ITimeZoneResolver tzResolver, 
  IMapper mapper
): IRequestHandler<GetLogEjecucionHistoryQuery, Result<WithMeta<PageResponse<LogEjecucionHistoryItemResponse>, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<PageResponse<LogEjecucionHistoryItemResponse>, DateConversionMeta>>> Handle(GetLogEjecucionHistoryQuery request, CancellationToken ct)
  {
    var (items, total) = await repo.GetHistoryAsync(
      request.Page,
      request.PageSize,
      request.SortBy,
      request.SortDir,
      request.confirmado,
      request.CreadoDelUtc,
      request.CreadoAlUtc,
      request.EstadoId,
      request.Codigo,
      request.Contrato,
      ct
    );

    var dtoItems = mapper.Map<List<LogEjecucionHistoryItemResponse>>(items);

    var pageResp = PageResponse<LogEjecucionHistoryItemResponse>.Of(dtoItems, request.Page, request.PageSize, total);

    var payload = DateConversionComposer.Wrap(pageResp, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<PageResponse<LogEjecucionHistoryItemResponse>, DateConversionMeta>>.Ok(payload);
  }

}
