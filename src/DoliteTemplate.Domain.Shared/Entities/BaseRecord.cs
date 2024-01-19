namespace DoliteTemplate.Domain.Shared.Entities;

/// <summary>
///     基本记录
/// </summary>
public abstract class BaseRecord
{
    public DateTime Time { get; set; } = DateTime.UtcNow;
}