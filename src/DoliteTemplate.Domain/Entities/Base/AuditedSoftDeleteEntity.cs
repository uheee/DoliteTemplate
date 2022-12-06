namespace DoliteTemplate.Domain.Entities.Base;

public class AuditedSoftDeleteEntity : BaseEntity, IAudited, ISoftDelete
{
    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }

    public void CreateBy(Guid id)
    {
        CreatorId = id;
        CreationTime = DateTime.UtcNow;
    }

    public void ModifyBy(Guid id)
    {
        LastModifierId = id;
        LastModificationTime = DateTime.UtcNow;
    }

    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void DeleteBy(Guid id)
    {
        Delete();
        DeleterId = id;
        DeletionTime = DateTime.UtcNow;
    }
}