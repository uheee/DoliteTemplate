using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoliteTemplate.Domain.Shared.Utils;

/// <summary>
///     日期时间类型的JSON转换器
/// </summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var time = reader.GetDateTime();
        // 用于将未指定本地/UTC的日期时间类型指定为本地时间
        if (time.Kind == DateTimeKind.Unspecified)
        {
            time = DateTime.SpecifyKind(time, DateTimeKind.Local);
        }

        return time;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocalTime());
    }
}