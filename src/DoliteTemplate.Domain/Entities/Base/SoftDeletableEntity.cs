using System.ComponentModel.DataAnnotations.Schema;

namespace DoliteTemplate.Domain.Entities.Base;

public class SoftDeletableEntity : BaseEntity, ISoftDeletable
{
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