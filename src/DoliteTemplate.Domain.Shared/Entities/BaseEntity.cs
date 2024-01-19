using System.ComponentModel.DataAnnotations;

namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     基本实体
/// </summary>
public class BaseEntity
{
    /// <summary>
    ///     实体唯一标识
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}