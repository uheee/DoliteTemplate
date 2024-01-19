namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     查询参数注解
///     <remarks>标记实体内的属性，将自动为被标记的属性生成Restful API的Query查询参数</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class QueryParameterAttribute : Attribute
{
    /// <summary>
    ///     查询参数名称
    ///     <remarks>未设置时默认为属性名的驼峰命名方式</remarks>
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     导航属性
    ///     <remarks>用于将查询参数引导至嵌套实体内部</remarks>
    ///     <example><![CDATA[
    /// 如存在：
    /// ```C#
    /// public class Foo
    /// {
    ///     [QueryParameter("InnerContent")]
    ///     public Bar SomaBar { get; set; }
    /// }
    /// public class Bar
    /// {
    ///     public string InnerContent { get; set; }
    /// }
    /// ```
    /// 将生成可用于查询`Bar`的`InnerContent`属性的查询参数，默认名为`someBarInnerContent`。
    /// ]]></example>
    /// </summary>
    public string? Navigation { get; set; }

    /// <summary>
    ///     实体参数与用户输入的查询参数的比较
    ///     <remarks><![CDATA[
    /// 该值为string类型，其约定值有以下几种模板：
    /// - 不填，行为与等于eq相同
    /// - eq，对两个值作 {0} == {1} 比较
    /// - lt，对两个值作 {0} < {1} 比较
    /// - gt，对两个值作 {0} > {1} 比较
    /// - lte，对两个值作 {0} <= {1} 比较
    /// - gte，对两个值作 {0} >= {1} 比较
    /// - contains，对两个值作 {0}.Contains({1}) 比较（仅字符串类型有效）
    /// - 其他输入，将使用输入参数作为string.Format的第一个参数，实体属性作为第二个参数，用户输入参数作为第三个参数生成的语句作为比较方法
    /// ]]></remarks>
    /// </summary>
    public string? Comparor { get; set; }

    /// <summary>
    ///     默认值
    ///     <remarks>用户不输入时的默认参数值</remarks>
    /// </summary>
    public object? Default { get; set; }

    /// <summary>
    ///     空参数忽略
    ///     <remarks>若该值为True，当用户未输入指定的查询参数值时，不对该参数进行查询筛选</remarks>
    /// </summary>
    public bool IgnoreWhenNull { get; set; }

    /// <summary>
    ///     查询参数描述
    /// </summary>
    public string? Description { get; set; }
}