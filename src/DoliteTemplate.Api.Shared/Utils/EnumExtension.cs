using System.ComponentModel;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     枚举扩展类
/// </summary>
public static class EnumExtension
{
    /// <summary>
    ///     获取枚举的描述信息
    /// </summary>
    public static string GetDescription(this Enum em)
    {
        var type = em.GetType();
        var fd = type.GetField(em.ToString());
        if (fd == null)
        {
            return string.Empty;
        }

        var attrs = fd.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var name = string.Empty;
        foreach (DescriptionAttribute attr in attrs)
        {
            name = attr.Description;
        }

        return name;
    }
}