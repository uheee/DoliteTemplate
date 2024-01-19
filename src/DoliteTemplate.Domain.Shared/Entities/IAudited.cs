namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     审计特性
/// </summary>
public interface IAudited
{
    /// <summary>
    ///     创建者唯一标识
    /// </summary>
    Guid? CreatorId { get; set; }

    /// <summary>
    ///     创建时间
    /// </summary>
    DateTime? CreationTime { get; set; }

    /// <summary>
    ///     最后修改者唯一标识
    /// </summary>
    Guid? LastModifierId { get; set; }

    /// <summary>
    ///     最后修改时间
    /// </summary>
    DateTime? LastModificationTime { get; set; }

    /// <summary>
    ///     附加创建者信息
    /// </summary>
    /// <param name="id">创建者唯一标识</param>
    public void CreateBy(Guid? id)
    {
        CreatorId = id;
        CreationTime = DateTime.UtcNow;
    }

    /// <summary>
    ///     附加修改者信息
    /// </summary>
    /// <param name="id">修改者唯一标识</param>
    public void ModifyBy(Guid? id)
    {
        LastModifierId = id;
        LastModificationTime = DateTime.UtcNow;
    }
}