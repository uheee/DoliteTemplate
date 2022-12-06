namespace DoliteTemplate.Domain.Entities.Base;

public class SoftDeleteEntity : BaseEntity, ISoftDelete
{
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