namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     JWT Claim键名
/// </summary>
public static class ClaimKeys
{
    /// <summary>
    ///     前缀
    /// </summary>
    public const string Prefix = "DoliteTemplate";

    /// <summary>
    ///     用户唯一标识
    /// </summary>
    public const string UserId = $"{Prefix}:uid";

    /// <summary>
    ///     角色
    /// </summary>
    public const string Role = $"{Prefix}:role";
}