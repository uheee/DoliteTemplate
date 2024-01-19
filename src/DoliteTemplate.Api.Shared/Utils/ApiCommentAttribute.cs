namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     OpenAPI 注释文档
///     <remarks>适用于跨项目的注释无法被源生成器正确提取</remarks>
///     <example>在方法、方法参数上以<b>[ApiComment("说明内容")]</b>的方式标记</example>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
public class ApiCommentAttribute : Attribute
{
    /// <summary>
    ///     构造OpenAPI 注释文档
    /// </summary>
    /// <param name="comment">注释内容</param>
    public ApiCommentAttribute(string comment)
    {
        Comment = comment;
    }

    /// <summary>
    ///     注释内容
    /// </summary>
    public string Comment { get; }
}