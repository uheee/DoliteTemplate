using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class CrudService<TService, TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto> :
    BaseService<TService, TDbContext>,
    ICrudService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    /// <summary>
    ///     Get entity by identifier
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns>An entity</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<TReadDto?> Get(Guid id)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .SingleOrDefaultAsync(entity => entity.Id == id);
        return Mapper.Map<TReadDto>(result);
    }

    /// <summary>
    ///     Get all entities
    /// </summary>
    /// <returns>All entities</returns>
    [HttpGet]
    public async Task<IEnumerable<TReadDto>> GetAll()
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToArrayAsync();
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
    public async Task<PaginatedList<TReadDto>> GetAllPaged(int pageIndex, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TReadDto>>(result);
    }

    /// <summary>
    ///     Create an entity
    /// </summary>
    /// <param name="dto">entity DTO</param>
    /// <returns>The created entity</returns>
    [HttpPost]
    public async Task<TReadDto?> Create([FromBody] TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

    /// <summary>
    ///     Update an entity
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="dto">New entity DTO</param>
    /// <returns>The updated entity</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<TReadDto?> Update(Guid id, [FromBody] TUpdateDto dto)
    {
        var entity = new TEntity { Id = id };
        DbContext.Set<TEntity>().Attach(entity);
        Mapper.Map(dto, entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

    /// <summary>
    ///     Delete an entity
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns>The deleted entity</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<TReadDto> Delete(Guid id)
    {
        var entity = new TEntity { Id = id };
        DbContext.Set<TEntity>().Attach(entity);
        TEntity result;
        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.Delete();
                result = entity;
                break;
            default:
                result = DbContext.Set<TEntity>().Remove(entity).Entity;
                break;
        }

        await DbContext.SaveChangesAsync();
        return Mapper.Map<TReadDto>(result);
    }

    /// <summary>
    ///     Delete some entities
    /// </summary>
    /// <param name="ids">Unique identifiers</param>
    /// <returns>Count of the deleted entities</returns>
    [HttpDelete]
    public async Task<int> DeleteRange([FromBody] IEnumerable<Guid> ids)
    {
        var entities = ids.Select(id => new TEntity { Id = id }).ToList();
        DbContext.Set<TEntity>().AttachRange(entities);
        if (typeof(TEntity).IsAssignableTo(typeof(ISoftDelete)))
            entities.ForEach(entity => ((ISoftDelete)entity).Delete());
        else
            DbContext.Set<TEntity>().RemoveRange(entities);
        var result = await DbContext.SaveChangesAsync();
        return result;
    }
}

public class CrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto> :
    CrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>
    where TService : CrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}

public class CrudService<TService, TDbContext, TEntity, TDto> :
    CrudService<TService, TDbContext, TEntity, TDto, TDto, TDto>
    where TService : CrudService<TService, TDbContext, TEntity, TDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}