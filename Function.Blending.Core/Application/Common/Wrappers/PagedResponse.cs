namespace Function.Blending.Core.Application.Common.Wrappers;

public class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = Array.Empty<T>();
    public PaginationInfo Pagination { get; init; } = new();
}

public class PaginationInfo
{
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }
    public int? PreviousPage { get; init; }
    public int? NextPage { get; init; }
}
