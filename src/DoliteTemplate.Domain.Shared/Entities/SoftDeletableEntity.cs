using System.ComponentModel.DataAnnotations.Schema;

namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     附带软删除信息的实体
/// </summary>
public class SoftDeletableEntity : BaseEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
    [NotMapped] public bool IsHardDeleted { get; set; }
}