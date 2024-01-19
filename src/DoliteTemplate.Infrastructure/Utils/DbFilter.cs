namespace DoliteTemplate.Infrastructure.Utils;

/// <summary>
///     数据库过滤器描述
///     <remarks>指定何种情况时，允许插入新实体</remarks>
/// </summary>
public class DbFilter
{
    /// <summary>
    ///     过滤模式
    /// </summary>
    public DbFilterMode Mode { get; set; }

    /// <summary>
    ///     模式参数
    /// </summary>
    public string? Args { get; set; }
}

/// <summary>
///     过滤模式
/// </summary>
public enum DbFilterMode
{
    /// <summary>
    ///     任何情况都允许插入新实体
    /// </summary>
    Always,

    /// <summary>
    ///     仅当实体对应的数据库表为空时允许插入新实体
    /// </summary>
    Empty,

    /// <summary>
    ///     仅当实体对应的数据库表不存在与要插入的实体含有相同的指定键时允许插入新实体
    ///     <remarks>指定的键名由<see cref="DbFilter.Args" />确定</remarks>
    /// </summary>
    Key,

    /// <summary>
    ///     任何情况都不允许插入新实体
    /// </summary>
    Never
}