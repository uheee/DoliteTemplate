using DoliteTemplate.Api.Shared.Utils;
using DoliteTemplate.Domain.Shared.Entities;

namespace DoliteTemplate.Domain.Entities;

/// <summary>
///     订单实体
/// </summary>
public class Order : AuditedSoftDeletableEntity
{
    /// <summary>
    ///     订单编号
    /// </summary>
    [QueryParameter(Description = "This is NO!", Comparor = "contains")]
    public string No { get; set; } = null!;
}