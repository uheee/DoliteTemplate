namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     软删除特性
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    ///     软删除特性的数据库属性名称集合
    /// </summary>
    public static string[] DbProperties { get; } = { nameof(IsDeleted), nameof(DeletionTime), nameof(DeleterId) };

    /// <summary>
    ///     是否被删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     删除者唯一标识
    /// </summary>
    public Guid? DeleterId { get; set; }

    /// <summary>
    ///     删除时间
    /// </summary>
    public DateTime? DeletionTime { get; set; }

    /// <summary>
    ///     物理删除标记
    ///     <para>如果该属性为True，将执行物理删除</para>
    /// </summary>
    public bool IsHardDeleted { get; set; }

    /// <summary>
    ///     附加删除信息
    /// </summary>
    /// <param name="hard">是否为物理删除</param>
    public void Delete(bool hard = false)
    {
        IsHardDeleted = hard;
        IsDeleted = true;
        DeletionTime = DateTime.UtcNow;
    }

    /// <summary>
    ///     附加删除信息
    /// </summary>
    /// <param name="id">删除者唯一标识</param>
    /// <param name="hard">是否为物理删除</param>
    public void DeleteBy(Guid? id, bool hard = false)
    {
        Delete(hard);
        DeleterId = id;
    }
}