namespace Function.Blending.Opt.Application.Common.Paging;

public sealed class PageRequest
{
  public int Page { get; init; } = 1;
  public int PageSize { get; init; } = 50;

  public int ClampPage(int min = 1) => Page < min ? min : Page;
  public int ClampPageSize(int min = 1, int max = 200) =>
      PageSize < min ? min : (PageSize > max ? max : PageSize);
}
