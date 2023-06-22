namespace DoliteTemplate.Shared.Utils;

public static class ByteExtensions
{
    public static byte[] TrimZero(this byte[] content)
    {
        var lastIndex = Array.FindLastIndex(content, b => b != 0);
        Array.Resize(ref content, lastIndex + 1);
        return content;
    }
}