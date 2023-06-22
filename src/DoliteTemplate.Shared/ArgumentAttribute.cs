namespace DoliteTemplate.Shared;

[AttributeUsage(AttributeTargets.Property)]
public class ArgumentAttribute : Attribute
{
    public ArgumentType Type { get; set; }
    public dynamic? Default { get; set; }
}

public enum ArgumentType
{
    Object,
    String,
    Number,
    Boolean
}