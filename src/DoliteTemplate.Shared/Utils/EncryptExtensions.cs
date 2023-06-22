using System.Security.Cryptography;
using System.Text;

namespace DoliteTemplate.Shared.Utils;

public static class EncryptExtensions
{
    public const int SaltLength = 64;

    public static byte[] GetRandomSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltLength);
    }

    public static byte[] Sha3(this byte[] content)
    {
        return SHA3.Net.Sha3.Sha3512().ComputeHash(content);
    }

    public static byte[] Sha3(this byte[] content, byte[] salt)
    {
        return SHA3.Net.Sha3.Sha3512().ComputeHash(content.Concat(salt).ToArray());
    }

    public static byte[] Sha3(this string raw)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);
        return SHA3.Net.Sha3.Sha3512().ComputeHash(rawBytes);
    }

    public static byte[] Sha3(this string raw, byte[] salt)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);
        return SHA3.Net.Sha3.Sha3512().ComputeHash(rawBytes.Concat(salt).ToArray());
    }

    public static async Task<byte[]> ComputeHash(this Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        return await SHA3.Net.Sha3.Sha3512().ComputeHashAsync(stream);
    }
}