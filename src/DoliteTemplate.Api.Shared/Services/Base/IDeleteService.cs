namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     删除实体特性
/// </summary>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
public interface IDeleteService<in TKey>
{
    /// <summary>
    ///     删除单个实体
    /// </summary>
    /// <param name="id">实体唯一标识</param>
    /// <returns>影响数量</returns>
    Task<int> Delete(TKey id);

    /// <summary>
    ///     删除多个实体
    /// </summary>
    /// <param name="ids">实体唯一标识</param>
    /// <returns>影响数量</returns>
    Task<int> DeleteRange(IEnumerable<TKey> ids);
}