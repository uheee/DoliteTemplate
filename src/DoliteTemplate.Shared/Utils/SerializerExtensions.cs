using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace DoliteTemplate.Shared.Utils;

public static class SerializerExtensions
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } =
        new JsonSerializerOptions().ConfigureDefault();

    public static JsonSerializerOptions ConfigureDefault(this JsonSerializerOptions options)
    {
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    public static JsonSerializerOptions CustomizeJsonSerializerOptions(Action<JsonSerializerOptions> customization)
    {
        var @default = new JsonSerializerOptions();
        customization(@default);
        return @default;
    }

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

    public static string ToCamelCase(this string content)
    {
        return content switch
        {
            { Length: 0 } => string.Empty,
            { Length: 1 } => content.ToLower(),
            { Length: > 1 } => content[..1].ToLower() + content[1..]
        };
    }
}