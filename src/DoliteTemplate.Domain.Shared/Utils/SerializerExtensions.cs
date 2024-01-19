using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace DoliteTemplate.Domain.Shared.Utils;

/// <summary>
///     序列化扩展
/// </summary>
public static class SerializerExtensions
{
    /// <summary>
    ///     开箱即用的JSON序列化配置
    /// </summary>
    public static JsonSerializerOptions JsonSerializerOptions { get; } =
        new JsonSerializerOptions().ConfigureDefault();

    /// <summary>
    ///     配置默认JSON序列化风格
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static JsonSerializerOptions ConfigureDefault(this JsonSerializerOptions options)
    {
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new DateTimeConverter());
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        return options;
    }

    /// <summary>
    ///     自定义JSON序列化风格
    /// </summary>
    /// <param name="customization">自定义内容</param>
    /// <returns></returns>
    public static JsonSerializerOptions CustomizeJsonSerializerOptions(Action<JsonSerializerOptions> customization)
    {
        var @default = new JsonSerializerOptions();
        customization(@default);
        return @default;
    }

    /// <summary>
    ///     配置项转换为JsonNode类型
    /// </summary>
    /// <param name="configuration">配置项</param>
    /// <returns>JsonNode结果</returns>
    public static JsonNode? ToJsonNode(this IConfiguration? configuration)
    {
        if (configuration is null)
        {
            return null;
        }

        JsonObject obj = new();
        foreach (var child in configuration.GetChildren())
        {
            if (child.Path.EndsWith(":0"))
            {
                var arr = new JsonArray();
                foreach (var arrayChild in configuration.GetChildren())
                {
                    arr.Add(arrayChild.ToJsonNode());
                }

                return arr;
            }

            obj.Add(child.Key, child.ToJsonNode());
        }

        if (obj.Any() || configuration is not IConfigurationSection section)
        {
            return obj;
        }

        if (bool.TryParse(section.Value, out var boolean))
        {
            return JsonValue.Create(boolean);
        }

        if (decimal.TryParse(section.Value, out var real))
        {
            return JsonValue.Create(real);
        }

        if (long.TryParse(section.Value, out var integer))
        {
            return JsonValue.Create(integer);
        }

        return JsonValue.Create(section.Value);
    }
}