using AutoMapper;
using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetHistory;

public sealed class GetCalEjecucionHistoryQueryHandler(ICalEjecucionRepository repo, IMapper mapper)
  : IRequestHandler<GetCalEjecucionHistoryQuery, Result<PageResponse<CalEjecucionHistoryItemResponse>>>
{
  public async Task<Result<PageResponse<CalEjecucionHistoryItemResponse>>> Handle(GetCalEjecucionHistoryQuery request, CancellationToken ct)
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

    return Result<PageResponse<CalEjecucionHistoryItemResponse>>.Ok(pageResp);
  }

}
