namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     <b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TOverallDto">查看实体相关概览DTO类型</typeparam>
/// <typeparam name="TDetailDto">查看实体相关详细DTO类型</typeparam>
/// <typeparam name="TCreateDto">创建实体相关DTO类型</typeparam>
/// <typeparam name="TUpdateDto">更新实体相关DTO类型</typeparam>
public interface ICrudService<TEntity, in TKey, TOverallDto, TDetailDto, in TCreateDto, in TUpdateDto> :
    IReadService<TKey, TOverallDto, TDetailDto>,
    ICreateService<TCreateDto, TDetailDto>,
    IUpdateService<TKey, TUpdateDto>,
    IDeleteService<TKey>;

/// <summary>
///     <b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TReadDto">查看实体相关DTO类型</typeparam>
/// <typeparam name="TCreateDto">创建实体相关DTO类型</typeparam>
/// <typeparam name="TUpdateDto">更新实体相关DTO类型</typeparam>
public interface ICrudService<TEntity, in TKey, TReadDto, in TCreateDto, in TUpdateDto> :
    ICrudService<TEntity, TKey, TReadDto, TReadDto, TCreateDto, TUpdateDto>;

/// <summary>
///     <b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TReadDto">查看实体相关DTO类型</typeparam>
/// <typeparam name="TCreateUpdateDto">创建、更新实体相关DTO类型</typeparam>
public interface ICrudService<TEntity, in TKey, TReadDto, in TCreateUpdateDto> :
    ICrudService<TEntity, TKey, TReadDto, TCreateUpdateDto, TCreateUpdateDto>;

/// <summary>
///     <b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
/// <typeparam name="TDto">实体相关DTO类型</typeparam>
public interface ICrudService<TEntity, in TKey, TDto> :
    ICrudService<TEntity, TKey, TDto, TDto>;