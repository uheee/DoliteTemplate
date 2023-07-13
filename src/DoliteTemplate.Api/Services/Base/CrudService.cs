using System.Reflection;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Infrastructure.Utils;
using DoliteTemplate.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DoliteTemplate.Api.Services.Base;

public abstract class CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> :
    BaseService<TService, TDbContext>,
    ICrudService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : class, new()
{
    /// <summary>
    ///     Get entity by identifier
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns>An entity</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public virtual async Task<TReadDto?> Get(TKey id)
    {
        var result = await DbContext.Set<TEntity>()
            .FindAsync(id);
        return Mapper.Map<TReadDto>(result);
    }

    /// <summary>
    ///     Get all entities
    /// </summary>
    /// <returns>All entities</returns>
    [HttpGet]
    public virtual async Task<IEnumerable<TReadDto>> GetAll()
    {
        IQueryable<TEntity> query = DbContext.Set<TEntity>();
        query = AfterQuery(query);
        var result = await query.ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    /// <summary>
    ///     Get all paginated entities
    /// </summary>
    /// <param name="pageIndex">Page index (start from 1)</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>All paginated entities</returns>
    [HttpGet]
    [Route("paging")]
    public virtual async Task<PaginatedList<TReadDto>> GetAllPaged(int pageIndex, int pageSize)
    {
        IQueryable<TEntity> query = DbContext.Set<TEntity>();
        query = AfterQuery(query);
        var result = await query.ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TReadDto>>(result);
    }

    /// <summary>
    ///     Create an entity
    /// </summary>
    /// <param name="dto">entity DTO</param>
    /// <returns>The created entity</returns>
    [HttpPost]
    public virtual async Task<TReadDto> Create([FromBody] TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return Mapper.Map<TReadDto>(entity);
    }

    public abstract Task<int> Update(TKey id, TUpdateDto dto);

    public abstract Task<int> Delete(TKey id);

    public abstract Task<int> DeleteRange(IEnumerable<TKey> ids);

    protected virtual IQueryable<TEntity> AfterQuery(IQueryable<TEntity> query)
    {
        return query;
    }
}

public abstract class CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateUpdateDto> :
    CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateUpdateDto, TCreateUpdateDto>,
    ICrudService<TEntity, TKey, TReadDto, TCreateUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : class, new()
{
}

public abstract class CrudService<TService, TDbContext, TEntity, TKey, TDto> :
    CrudService<TService, TDbContext, TEntity, TKey, TDto, TDto, TDto>,
    ICrudService<TEntity, TKey, TDto>
    where TService : CrudService<TService, TDbContext, TEntity, TKey, TDto>
    where TDbContext : DbContext
    where TEntity : class, new()
{
}