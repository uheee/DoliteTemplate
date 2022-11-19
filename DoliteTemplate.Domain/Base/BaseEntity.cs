using System.ComponentModel.DataAnnotations;

namespace DoliteTemplate.Domain.Base;

public class BaseEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
}