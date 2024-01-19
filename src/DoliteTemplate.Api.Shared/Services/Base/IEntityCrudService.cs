using DoliteTemplate.Domain.Shared.Entities;

namespace DoliteTemplate.Api.Shared.Services.Base;

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TOverallDto">查看实体相关概览DTO类型</typeparam>
/// <typeparam name="TDetailDto">查看实体相关详细DTO类型</typeparam>
/// <typeparam name="TCreateDto">创建实体相关DTO类型</typeparam>
/// <typeparam name="TUpdateDto">更新实体相关DTO类型</typeparam>
public interface IEntityCrudService<TEntity, TOverallDto, TDetailDto, in TCreateDto, in TUpdateDto> :
    ICrudService<TEntity, Guid, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>;

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TReadDto">查看实体相关DTO类型</typeparam>
/// <typeparam name="TCreateDto">创建实体相关DTO类型</typeparam>
/// <typeparam name="TUpdateDto">更新实体相关DTO类型</typeparam>
public interface IEntityCrudService<TEntity, TReadDto, in TCreateDto, in TUpdateDto> :
    IEntityCrudService<TEntity, TReadDto, TReadDto, TCreateDto, TUpdateDto>;

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TReadDto">查看实体相关DTO类型</typeparam>
/// <typeparam name="TCreateUpdateDto">创建、更新实体相关DTO类型</typeparam>
public interface IEntityCrudService<TEntity, TReadDto, in TCreateUpdateDto> :
    IEntityCrudService<TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>;

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>实体特性
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TDto">实体相关DTO类型</typeparam>
public interface IEntityCrudService<TEntity, TDto> :
    IEntityCrudService<TEntity, TDto, TDto>;