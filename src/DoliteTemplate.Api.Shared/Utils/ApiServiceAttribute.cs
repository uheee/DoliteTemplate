using DoliteTemplate.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     API服务注解
///     <remarks>该Attribute标记的<see cref="BaseService" />派生类</remarks>
///     <remarks>将被源生成器捕获并生成对应的<see cref="ControllerBase" />派生类，</remarks>
///     <remarks>自动根据该类中标记了<see cref="HttpMethodAttribute" />派生Attribute的方法。</remarks>
///     <remarks>生成对应的HTTP方法</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ApiServiceAttribute : Attribute
{
    /// <summary>
    ///     服务标签
    ///     <remarks>用于确定生成的<see cref="ControllerBase" />派生类名称，固定生成为<b>[Tag]Controller</b>。</remarks>
    ///     <remarks>未指定时，如果该Attribute标记的类名为<b>[Name]Service</b>的格式，则<b>Tag=Name</b>；</remarks>
    ///     <remarks>否则Tag为类名。</remarks>
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    ///     生成方法规则
    ///     <remarks>指定需要生成为HTTP方法的C#方法，仅当C#方法名匹配该值指定正则表达时才会生成基于该C#方法的HTTP方法。</remarks>
    /// </summary>
    public string Rule { get; init; } = ".*";
}