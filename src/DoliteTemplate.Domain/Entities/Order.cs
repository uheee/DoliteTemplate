using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Domain.Entities;

public class Order : AuditedSoftDeletableEntity
{
    [QueryParameter(Name = "no")]
    public string No { get; set; } = null!;
}