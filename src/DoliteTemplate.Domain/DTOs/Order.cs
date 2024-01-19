namespace DoliteTemplate.Domain.DTOs;

/// <summary>
///     查看订单DTO
/// </summary>
public class OrderReadDto
{
    /// <summary>
    ///     订单唯一标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     订单编号
    /// </summary>
    public string No { get; set; } = null!;
}

/// <summary>
///     创建订单DTO
/// </summary>
public class OrderCreateDto
{
    /// <summary>
    ///     订单编号
    /// </summary>
    public string No { get; set; } = null!;
}

/// <summary>
///     更新订单DTO
/// </summary>
public class OrderUpdateDto
{
    /// <summary>
    ///     订单编号
    /// </summary>
    public string No { get; set; } = null!;
}