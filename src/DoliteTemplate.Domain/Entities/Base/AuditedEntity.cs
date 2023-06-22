namespace DoliteTemplate.Domain.Entities.Base;

public class AuditedEntity : BaseEntity, IAudited
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
}