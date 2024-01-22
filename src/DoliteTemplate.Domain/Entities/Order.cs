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
    public string No { get; set; } = null!;
}