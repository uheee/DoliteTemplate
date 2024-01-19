using DoliteTemplate.Domain.Shared.Utils;

namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     查看实体特性
/// </summary>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TOverallDto">查看实体相关概览DTO类型</typeparam>
/// <typeparam name="TDetailDto">查看实体相关详细DTO类型</typeparam>
public interface IReadService<in TKey, TOverallDto, TDetailDto>
{
    /// <summary>
    ///     查看单个实体
    /// </summary>
    /// <param name="id">实体唯一标识</param>
    /// <returns>查看实体相关详细DTO</returns>
    Task<TDetailDto?> Get(TKey id);

    /// <summary>
    ///     查看所有实体
    /// </summary>
    /// <returns>查看实体相关概览DTO</returns>
    Task<IEnumerable<TOverallDto>> GetAll();

    /// <summary>
    ///     <b>分页</b>查看所有实体
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>查看实体相关概览DTO</returns>
    Task<PaginatedList<TOverallDto>> GetAllPaged(int pageIndex, int pageSize);
}