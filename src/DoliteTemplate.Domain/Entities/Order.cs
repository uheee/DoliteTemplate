using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Shared;

namespace DoliteTemplate.Domain.Entities;

public class Order : AuditedSoftDeletableEntity
{
    [QueryParameter(Description = "This is NO!", Comparor = "contains")]
    public string No { get; set; } = null!;
}