using System.ComponentModel.DataAnnotations.Schema;

namespace DoliteTemplate.Domain.Entities.Base;

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

    public void Delete(bool hard)
    {
        IsHardDeleted = hard;
        IsDeleted = true;
        DeletionTime = DateTime.UtcNow;
    }

    public void DeleteBy(Guid? id, bool hard)
    {
        Delete(hard);
        DeleterId = id;
    }
}