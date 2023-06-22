namespace DoliteTemplate.Domain.Entities.Base;

public abstract class BaseRecord
{
    public DateTime Time { get; set; } = DateTime.UtcNow;
}