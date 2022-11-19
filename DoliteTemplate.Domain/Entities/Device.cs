using DoliteTemplate.Domain.Base;

namespace DoliteTemplate.Domain.Entities;

public class Device : AuditedSoftDeleteEntity
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
}