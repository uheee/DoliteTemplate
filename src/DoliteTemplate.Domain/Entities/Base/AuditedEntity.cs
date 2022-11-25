namespace DoliteTemplate.Domain.Entities.Base;

public class AuditedEntity : BaseEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}