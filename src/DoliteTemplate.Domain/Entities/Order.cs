using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Shared;

namespace DoliteTemplate.Domain.Entities;

public class Order : AuditedSoftDeletableEntity
{
    [QueryParameter(Name = "no")]
    public string No { get; set; } = null!;
}