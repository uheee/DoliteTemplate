namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     创建实体特性
/// </summary>
/// <typeparam name="TCreateDto">创建实体相关DTO类型</typeparam>
/// <typeparam name="TReadDto">查看实体相关DTO类型</typeparam>
public interface ICreateService<in TCreateDto, TReadDto>
{
    /// <summary>
    ///     创建实体
    /// </summary>
    /// <param name="dto">创建实体相关DTO</param>
    /// <returns>查看实体相关DTO</returns>
    Task<TReadDto> Create(TCreateDto dto);
}