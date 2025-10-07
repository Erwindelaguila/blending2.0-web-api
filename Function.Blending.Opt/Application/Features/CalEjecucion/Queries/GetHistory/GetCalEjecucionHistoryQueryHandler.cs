using AutoMapper;
using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Application.Support.Time;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetHistory;

public sealed class GetCalEjecucionHistoryQueryHandler(
  ICalEjecucionRepository repo,
  ITimeZoneService tz,
  ITimeZoneResolver tzResolver, 
  IMapper mapper
) : IRequestHandler<GetCalEjecucionHistoryQuery, Result<WithMeta<PageResponse<CalEjecucionHistoryItemResponse>, DateConversionMeta>>>
{
  public async Task<Result<WithMeta<PageResponse<CalEjecucionHistoryItemResponse>, DateConversionMeta>>> Handle(GetCalEjecucionHistoryQuery request, CancellationToken ct)
  {
    var (items, total) = await repo.GetHistoryAsync(
      request.Page,
      request.PageSize,
      request.SortBy,
      request.SortDir,
      request.CreadoDelUtc,
      request.CreadoAlUtc,
      request.EstadoId,
      request.PlantaId,
      request.Codigo,
      ct
    );

    var dtoItems = mapper.Map<List<CalEjecucionHistoryItemResponse>>(items);

    var pageResp = PageResponse<CalEjecucionHistoryItemResponse>.Of(dtoItems, request.Page, request.PageSize, total);

    var payload = DateConversionComposer.Wrap(pageResp, request.ConvertDates, request.TzId, tz, tzResolver);
    return Result<WithMeta<PageResponse<CalEjecucionHistoryItemResponse>, DateConversionMeta>>.Ok(payload);
  }

}
