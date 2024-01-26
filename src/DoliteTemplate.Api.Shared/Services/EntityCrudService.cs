using AutoMapper;
using DoliteTemplate.Api.Shared.Services.Base;
using DoliteTemplate.Domain.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Shared.Services;

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="IEntityCrudService{TEntity,TOverallDto,TDetailDto,TCreateDto,TUpdateDto}" />
public class EntityCrudService<TService, TDbContext, TEntity, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    CrudService<TService, TDbContext, TEntity, Guid, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>(
        mapper,
        localizer,
        dbContextProvider),
    IEntityCrudService<TEntity, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    /// <inheritdoc />
    [HttpPut]
    [Route("{id:guid}")]
    public override async Task<int> Update(Guid id, TUpdateDto dto)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = QueryOneInclude(query);
        var entity = await query.SingleOrDefaultAsync(entity => entity.Id == id);
        if (entity is null)
        {
            return 0;
        }

        Mapper.Map(dto, entity);
        UpdateMarkDirtyManually(DbContext.Entry(entity), dto);
        return await DbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    [HttpDelete]
    [Route("{id:guid}")]
    public override async Task<int> Delete(Guid id)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = DeleteInclude(query);
        var entity = await query.SingleOrDefaultAsync(entity => entity.Id == id);
        if (entity is null)
        {
            throw Error("entity does not exist");
        }

        DbContext.Set<TEntity>().Remove(entity);
        return await DbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    [HttpDelete]
    public override async Task<int> DeleteRange(IEnumerable<Guid> ids)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = DeleteInclude(query);
        var entities = await query.Where(entity => ids.Contains(entity.Id)).ToArrayAsync();
        DbContext.Set<TEntity>().RemoveRange(entities);
        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     手动标记脏更新
    ///     <remarks>用于处理某些Map后无法自动变更属性IsModified属性的情况</remarks>
    ///     <example>通过<b>entry.Property(p => p.YourProperty).IsModified = true;</b>标记属性已被更改</example>
    /// </summary>
    /// <param name="entry">实体Entry</param>
    /// <param name="updateDto">更新实体相关DTO</param>
    [NonAction]
    protected virtual void UpdateMarkDirtyManually(EntityEntry<TEntity> entry, TUpdateDto updateDto)
    {
    }

    public override async ValueTask<TEntity?> GetRaw(Guid id)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = QueryOneInclude(query);
        var result = await query.SingleOrDefaultAsync(entity => entity.Id == id);
        return result;
    }

    public override async Task<TEntity> CreateRaw(TCreateDto dto)
    {
        var entity = await base.CreateRaw(dto);
        return (await GetRaw(entity.Id))!;
    }
}

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="IEntityCrudService{TEntity,TReadDto,TCreateDto,TUpdateDto}" />
public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreate, TUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    EntityCrudService<TService, TDbContext, TEntity, TReadDto, TReadDto, TCreate, TUpdateDto>(
        mapper,
        localizer,
        dbContextProvider),
    IEntityCrudService<TEntity, TReadDto, TCreate, TUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreate, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new();

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="IEntityCrudService{TEntity,TReadDto,TCreateUpdateDto}" />
public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>(
        mapper,
        localizer,
        dbContextProvider),
    IEntityCrudService<TEntity, TReadDto, TCreateUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new();

/// <summary>
///     基于<see cref="BaseEntity" />的<b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="IEntityCrudService{TEntity,TDto}" />
public class EntityCrudService<TService, TDbContext, TEntity, TDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    EntityCrudService<TService, TDbContext, TEntity, TDto, TDto, TDto>(
        mapper,
        localizer,
        dbContextProvider),
    IEntityCrudService<TEntity, TDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new();