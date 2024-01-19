using System.ComponentModel.DataAnnotations.Schema;

namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     附带审计与软删除信息的实体
/// </summary>
public class AuditedSoftDeletableEntity : BaseEntity, IAudited, ISoftDeletable
{
    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }

    public void CreateBy(Guid? id)
    {
        CreatorId = id;
        CreationTime = DateTime.UtcNow;
    }

    public void ModifyBy(Guid? id)
    {
        LastModifierId = id;
        LastModificationTime = DateTime.UtcNow;
    }

    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
    [NotMapped] public bool IsHardDeleted { get; set; }
}