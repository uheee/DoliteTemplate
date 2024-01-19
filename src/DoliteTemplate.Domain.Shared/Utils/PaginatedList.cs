namespace DoliteTemplate.Domain.Shared.Utils;

/// <summary>
///     分页列表
/// </summary>
/// <typeparam name="T">分页项类型</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    ///     构造分页列表
    /// </summary>
    /// <param name="content">分页项内容</param>
    /// <param name="count">分页项数量</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    public PaginatedList(IEnumerable<T> content, long count, long pageIndex, int pageSize)
    {
        Content = content;
        Count = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (count + pageSize) / pageSize;
    }

    /// <summary>
    ///     分页项数量
    /// </summary>
    public long Count { get; init; }

    /// <summary>
    ///     页码
    /// </summary>
    public long PageIndex { get; init; }

    /// <summary>
    ///     页大小
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    ///     总页数
    /// </summary>
    public long TotalPages { get; init; }

    /// <summary>
    ///     分页内容
    /// </summary>
    public IEnumerable<T> Content { get; init; }

    /// <summary>
    ///     是否存在上一页
    /// </summary>
    public bool HasPrevPage => PageIndex > 1;

    /// <summary>
    ///     是否存在下一页
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    ///     构造空分页列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <returns></returns>
    public static PaginatedList<T> Empty(long pageIndex, int pageSize)
    {
        return new PaginatedList<T>(Array.Empty<T>(), 0, pageIndex, pageSize);
    }
}