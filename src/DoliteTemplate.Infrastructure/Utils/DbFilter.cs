using DoliteTemplate.Shared;

namespace DoliteTemplate.Infrastructure.Utils;

public class DbFilter
{
    public DbFilterMode Mode { get; set; }
    public string? Args { get; set; }
}

public enum DbFilterMode
{
    Always,
    Empty,
    Key,
    Never
}