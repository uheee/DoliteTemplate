using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace DoliteTemplate.Domain.Shared.Utils;

/// <summary>
///     加密扩展
/// </summary>
public static class EncryptExtensions
{
    /// <summary>
    ///     生成随机盐
    /// </summary>
    /// <param name="saltLength">盐长度</param>
    /// <returns></returns>
    public static byte[] GetRandomSalt(int saltLength)
    {
        return RandomNumberGenerator.GetBytes(saltLength);
    }

    public static byte[] Sha3(this byte[] content)
    {
        return SHA3_512.HashData(content);
    }

    public static byte[] Sha3(this byte[] content, byte[] salt)
    {
        return SHA3_512.HashData(content.Concat(salt).ToArray());
    }

    public static byte[] Sha3(this string raw)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);
        return SHA3_512.HashData(rawBytes);
    }

    public static byte[] Sha3(this string raw, byte[] salt)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);
        return SHA3_512.HashData(rawBytes.Concat(salt).ToArray());
    }

    /// <summary>
    ///     计算哈希
    /// </summary>
    /// <param name="stream">来自文件或内存的流</param>
    /// <returns></returns>
    public static async Task<byte[]> ComputeHash(this Stream stream)
    {
        var position = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);
        var result = await SHA3_512.HashDataAsync(stream);
        stream.Position = position;
        return result;
    }

    /// <summary>
    ///     获取Argon2类型的哈希
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="salt">盐</param>
    /// <param name="memory">内存大小</param>
    /// <param name="parallelism">并行性</param>
    /// <param name="iterations">迭代次数</param>
    /// <param name="hashLength">哈希长度</param>
    /// <returns></returns>
    public static Task<byte[]> GetArgon2Hash(this byte[] password, byte[] salt, int memory, int parallelism,
        int iterations, int hashLength)
    {
        var argon = new Argon2id(password)
        {
            Salt = salt,
            MemorySize = memory,
            DegreeOfParallelism = parallelism,
            Iterations = iterations
        };
        return argon.GetBytesAsync(hashLength);
    }
}