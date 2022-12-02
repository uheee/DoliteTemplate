namespace DoliteTemplate.Domain.Utils;

public class PaginatedList<T>
{
    public PaginatedList(IEnumerable<T> content, long count, long pageIndex, int pageSize)
    {
        Content = content;
        Count = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (count + pageSize) / pageSize;
    }

    public long Count { get; init; }
    public long PageIndex { get; init; }
    public int PageSize { get; init; }
    public long TotalPages { get; init; }
    public IEnumerable<T> Content { get; init; }
    public bool HasPrevPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Empty(long pageIndex, int pageSize)
    {
        return new PaginatedList<T>(Array.Empty<T>(), 0, pageIndex, pageSize);
    }
}