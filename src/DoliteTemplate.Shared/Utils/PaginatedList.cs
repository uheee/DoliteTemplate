namespace DoliteTemplate.Shared.Utils;

public class PaginatedList<T>(IEnumerable<T> content, long count, long pageIndex, int pageSize)
{
    public long Count { get; init; } = count;
public long PageIndex { get; init; } = pageIndex;
public int PageSize { get; init; } = pageSize;
public long TotalPages { get; init; } = (count + pageSize) / pageSize;
public IEnumerable<T> Content { get; init; } = content;
public bool HasPrevPage => PageIndex > 1;
public bool HasNextPage => PageIndex < TotalPages;

public static PaginatedList<T> Empty(long pageIndex, int pageSize)
{
    return new PaginatedList<T>(Array.Empty<T>(), 0, pageIndex, pageSize);
}
}