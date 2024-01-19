namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     更新实体特性
/// </summary>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TUpdateDto">更新实体相关DTO类型</typeparam>
public interface IUpdateService<in TKey, in TUpdateDto>
{
    /// <summary>
    ///     更新实体
    /// </summary>
    /// <param name="id">实体唯一标识</param>
    /// <param name="dto">更新实体相关DTO</param>
    /// <returns></returns>
    Task<int> Update(TKey id, TUpdateDto dto);
}