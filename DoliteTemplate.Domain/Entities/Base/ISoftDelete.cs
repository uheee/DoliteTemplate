namespace DoliteTemplate.Domain.Entities.Base;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
}