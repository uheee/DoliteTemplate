namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     附带审计信息的实体
/// </summary>
public class AuditedEntity : BaseEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}