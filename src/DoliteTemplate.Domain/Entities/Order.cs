using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Shared;

namespace DoliteTemplate.Domain.Entities;

public class Order : AuditedSoftDeletableEntity
{
    [QueryParameter(Default = "hello", Description = "This is NO!")]
    public string No { get; set; } = null!;
}