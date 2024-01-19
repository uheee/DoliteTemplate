using System.Linq.Expressions;
using AutoMapper;
using DoliteTemplate.Api.Shared.Services.Base;
using DoliteTemplate.Api.Shared.Utils;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Shared.Services;

/// <summary>
///     增删改查服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="ICrudService{TEntity,TKey,TOverallDto,TDetailDto,TCreateDto,TUpdateDto}" />
public abstract class CrudService<
    TService, TDbContext, TEntity, TKey, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    BaseService<TService, TDbContext>(
        mapper,
        localizer,
        dbContextProvider),
    ICrudService<TEntity, TKey, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : class, new()
{
    #region Raw methods

    /// <summary>
    ///     查看单个实体
    /// </summary>
    /// <param name="id">实体唯一标识</param>
    /// <returns>实体</returns>
    public virtual ValueTask<TEntity?> GetRaw(TKey id)
    {
        return DbContext.Set<TEntity>().FindAsync(id);
    }

    /// <summary>
    ///     查看所有实体
    /// </summary>
    /// <returns>实体</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllRaw()
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = QueryManyInclude(query);
        return await query.ToArrayAsync();
    }

    /// <summary>
    ///     创建实体
    /// </summary>
    /// <param name="dto">创建实体相关DTO</param>
    /// <returns>实体</returns>
    public virtual async Task<TEntity> CreateRaw(TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await DbContext.Set<TEntity>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    #endregion

    #region HTTP methods

    [HttpGet]
    [Route("{id:guid}")]
    [ApiComment("查看单个实体")]
    public virtual async Task<TDetailDto?> Get(
        [ApiComment("实体唯一标识")] TKey id)
    {
        var entity = await GetRaw(id);
        return Mapper.Map<TDetailDto>(entity);
    }

    [HttpGet]
    [ApiComment("查看所有实体")]
    public virtual async Task<IEnumerable<TOverallDto>> GetAll()
    {
        var entities = await GetAllRaw();
        return Mapper.Map<IEnumerable<TOverallDto>>(entities);
    }

    [HttpGet]
    [Route("paging")]
    [ApiComment("分页查看所有实体")]
    public virtual async Task<PaginatedList<TOverallDto>> GetAllPaged(
        [ApiComment("页码")] int pageIndex,
        [ApiComment("页大小")] int pageSize)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = QueryManyInclude(query);
        var result = await query.ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TOverallDto>>(result);
    }

    public virtual async Task<IEnumerable<TEntity>> GetWhereRaw(Expression<Func<TEntity, bool>> predicate)
    {
        var query = DbContext.Set<TEntity>().AsTracking();
        query = QueryManyInclude(query);
        return await query.Where(predicate).ToArrayAsync();
    }

    [HttpPost]
    [ApiComment("创建实体")]
    public virtual async Task<TDetailDto> Create(
        [ApiComment("创建实体相关DTO")][FromBody] TCreateDto dto)
    {
        var entity = await CreateRaw(dto);
        return Mapper.Map<TDetailDto>(entity);
    }

    public abstract Task<int> Update(TKey id, TUpdateDto dto);

    public abstract Task<int> Delete(TKey id);

    public abstract Task<int> DeleteRange(IEnumerable<TKey> ids);

    #endregion

    #region Extensions

    /// <summary>
    ///     查询关联
    ///     <remarks>重写该方法以指定查询时需要关联的其他实体</remarks>
    /// </summary>
    /// <param name="query">查询</param>
    /// <returns>查询</returns>
    public virtual IQueryable<TEntity> QueryInclude(IQueryable<TEntity> query)
    {
        return query;
    }

    /// <summary>
    ///     指定单个查询时需要关联的其他实体
    ///     <remarks>重写该方法以指定查询时需要关联的其他实体</remarks>
    ///     <para>该方法默认与<see cref="QueryInclude" />保持一致</para>
    /// </summary>
    /// <inheritdoc cref="QueryInclude" />
    public virtual IQueryable<TEntity> QueryOneInclude(IQueryable<TEntity> query)
    {
        return QueryInclude(query);
    }

    /// <summary>
    ///     指定多个查询时需要关联的其他实体
    ///     <remarks>重写该方法以指定查询时需要关联的其他实体</remarks>
    ///     <para>该方法默认与<see cref="QueryInclude" />保持一致</para>
    /// </summary>
    /// <inheritdoc cref="QueryInclude" />
    public virtual IQueryable<TEntity> QueryManyInclude(IQueryable<TEntity> query)
    {
        return QueryInclude(query);
    }

    /// <summary>
    ///     指定删除时需要关联的其他实体
    ///     <remarks>重写该方法以指定删除时需要关联的其他实体</remarks>
    /// </summary>
    /// <inheritdoc cref="QueryInclude" />
    public virtual IQueryable<TEntity> DeleteInclude(IQueryable<TEntity> query)
    {
        return query;
    }

    #endregion
}

/// <summary>
///     <b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="ICrudService{TEntity,TKey,TReadDto,TCreateDto,TUpdateDto}" />
public abstract class CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TReadDto, TCreateDto, TUpdateDto>(
        mapper,
        localizer,
        dbContextProvider),
    ICrudService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : class, new();

/// <summary>
///     <b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="ICrudService{TEntity,TKey,TReadDto,TCreateUpdateDto}" />
public abstract class CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateUpdateDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TReadDto, TCreateUpdateDto, TCreateUpdateDto>(
        mapper,
        localizer,
        dbContextProvider),
    ICrudService<TEntity, TKey, TReadDto, TCreateUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : class, new();

/// <summary>
///     <b>增删改查</b>服务
/// </summary>
/// <inheritdoc cref="BaseService{TService,TDbContext}" />
/// <inheritdoc cref="ICrudService{TEntity,TKey,TDto}" />
public abstract class CrudService<TService, TDbContext, TEntity, TKey, TDto>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    CrudService<TService, TDbContext, TEntity, TKey, TDto, TDto, TDto, TDto>(
        mapper,
        localizer,
        dbContextProvider),
    ICrudService<TEntity, TKey, TDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TDto>
    where TDbContext : DbContext
    where TEntity : class, new();