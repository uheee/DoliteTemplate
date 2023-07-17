namespace DoliteTemplate.CodeGenerator;

public class Extensions
{
    public static string ToCamelCase(string content)
    {
        return content switch
        {
            { Length: 0 } => string.Empty,
            { Length: 1 } => content.ToLower(),
            { Length: > 1 } => content.Substring(0, 1).ToLower() + content.Substring(1)
        };
    }
}