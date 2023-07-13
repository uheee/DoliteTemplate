using DoliteTemplate.Domain.Entities.Base;

namespace DoliteTemplate.Domain.Entities;

public class Order : AuditedSoftDeletableEntity
{
    public string No { get; set; } = null!;
}