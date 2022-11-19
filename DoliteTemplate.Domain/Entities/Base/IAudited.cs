namespace DoliteTemplate.Domain.Entities.Base;

public interface IAudited
{
    Guid? CreatorId { get; set; }
    DateTime? CreationTime { get; set; }
    Guid? LastModifierId { get; set; }
    DateTime? LastModificationTime { get; set; }
}