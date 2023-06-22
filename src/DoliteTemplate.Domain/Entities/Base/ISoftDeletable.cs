namespace DoliteTemplate.Domain.Entities.Base;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
    public bool IsHardDeleted { get; set; }

    void Delete(bool hard = false);

    void DeleteBy(Guid? id, bool hard = false);
}