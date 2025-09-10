using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Common.Paging;

public sealed class PageResponse<T>
{
  public IReadOnlyList<T> Items { get; init; } = [];
  public int Page { get; init; }
  public int PageSize { get; init; }
  public int Total { get; init; }
  public int TotalPages => PageSize <= 0 ? 0 : (int)System.Math.Ceiling((double)Total / PageSize);

  public static PageResponse<T> Of(IReadOnlyList<T> items, int page, int pageSize, int total)
      => new() { Items = items, Page = page, PageSize = pageSize, Total = total };
}
