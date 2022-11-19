namespace Domain.Utils;

public class PagedList<T>
{
    public PagedList(IEnumerable<T> content, long itemCount, long index, int pageSize)
    {
        Content = content;
        ItemCount = itemCount;
        Index = index;
        PageSize = pageSize;
        PageCount = (itemCount + pageSize) / pageSize;
    }

    public long ItemCount { get; init; }
    public long Index { get; init; }
    public int PageSize { get; init; }
    public long PageCount { get; init; }
    public IEnumerable<T> Content { get; init; }

    public static PagedList<T> Empty(long index, int pageSize)
    {
        return new PagedList<T>(Array.Empty<T>(), 0, index, pageSize);
    }
}