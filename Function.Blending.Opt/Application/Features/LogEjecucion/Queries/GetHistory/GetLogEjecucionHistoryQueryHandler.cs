using AutoMapper;
using Function.Blending.Opt.Application.Common.Paging;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetHistory;

public sealed class GetLogEjecucionHistoryQueryHandler(ILogEjecucionRepository repo, IMapper mapper)
  : IRequestHandler<GetLogEjecucionHistoryQuery, Result<PageResponse<LogEjecucionHistoryItemResponse>>>
{
  public async Task<Result<PageResponse<LogEjecucionHistoryItemResponse>>> Handle(GetLogEjecucionHistoryQuery request, CancellationToken ct)
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
      ct
    );

    var dtoItems = mapper.Map<List<LogEjecucionHistoryItemResponse>>(items);

    var pageResp = PageResponse<LogEjecucionHistoryItemResponse>.Of(dtoItems, request.Page, request.PageSize, total);

    return Result<PageResponse<LogEjecucionHistoryItemResponse>>.Ok(pageResp);
  }

}
