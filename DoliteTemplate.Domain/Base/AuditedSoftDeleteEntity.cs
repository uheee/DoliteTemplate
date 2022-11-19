namespace DoliteTemplate.Domain.Base;

public class AuditedSoftDeleteEntity : BaseEntity, IAudited, ISoftDelete
{
    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
}