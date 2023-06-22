using DoliteTemplate.Shared;

namespace DoliteTemplate.Infrastructure.Utils;

public class DbFilter
{
    [Argument(Type = ArgumentType.String)] public DbFilterMode Mode { get; set; }
    [Argument(Type = ArgumentType.Object)] public string? Args { get; set; }
}

public enum DbFilterMode
{
    Always,
    Empty,
    Key,
    Never
}