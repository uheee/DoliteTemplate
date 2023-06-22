using DoliteTemplate.Domain.Entities.Base;

namespace DoliteTemplate.Domain.Entities;

public class Device : AuditedSoftDeletableEntity
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
}