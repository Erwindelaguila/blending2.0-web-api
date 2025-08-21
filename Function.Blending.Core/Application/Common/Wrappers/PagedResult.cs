using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.Common.Wrappers;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public int? PreviousPage => HasPrevious ? CurrentPage - 1 : null;
    public int? NextPage => HasNext ? CurrentPage + 1 : null;


    public static async Task<PagedResult<T>> CreateAsync(
        IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var count = await source.CountAsync(ct);
        var totalPages = count == 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);

        if (totalPages > 0 && page > totalPages) page = totalPages;

        var items = count == 0
            ? new List<T>()
            : await source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        return new PagedResult<T>
        {
            Items = items,
            CurrentPage = totalPages == 0 ? 1 : page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalCount = count,
        };
    }

    public PagedResponse<T> ToPagedResponse()
    {
        return new PagedResponse<T>
        {
            Data = Items,
            Pagination = new PaginationInfo
            {
                CurrentPage = CurrentPage,
                TotalPages = TotalPages,
                PageSize = PageSize,
                TotalCount = TotalCount,
                HasPrevious = HasPrevious,
                HasNext = HasNext,
                PreviousPage = PreviousPage,
                NextPage = NextPage
            }
        };
    }

}

public static class QueryablePagingExtensions
{
    public static Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken ct = default) =>
        PagedResult<T>.CreateAsync(source, page, pageSize, ct);
}
