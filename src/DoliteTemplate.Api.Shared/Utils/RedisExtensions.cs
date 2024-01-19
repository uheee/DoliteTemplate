using StackExchange.Redis;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     Redis扩展
/// </summary>
public static class RedisExtensions
{
    /// <summary>
    ///     扫描获取所有匹配的Redis键
    /// </summary>
    /// <param name="redis">Redis</param>
    /// <param name="pattern">匹配模式</param>
    /// <param name="database">数据库</param>
    /// <returns>键枚举异步流</returns>
    public static async IAsyncEnumerable<RedisScanItem> ScanKeys(this IConnectionMultiplexer redis,
        RedisValue pattern, int database = -1)
    {
        var endpoint = await redis.GetDatabase(database).IdentifyEndpointAsync();
        var server = redis.GetServer(endpoint!);
        await foreach (var redisKey in server.KeysAsync(pattern: pattern))
        {
            var key = redisKey.ToString();
            var sections = key.Split(':');
            yield return new RedisScanItem(redisKey, sections);
        }
    }
}

/// <summary>
///     Redis扫描项
/// </summary>
/// <param name="Key">键</param>
/// <param name="Sections">以:分隔的Redis节点</param>
public record RedisScanItem(RedisKey Key, string[] Sections);