using System.Linq.Expressions;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class CrudService<TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto> :
    BaseService<TDbContext>,
    ICrudService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<TReadDto> Get(Guid id)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .SingleAsync(entity => entity.Id == id);
        return Mapper.Map<TReadDto>(result);
    }

    [HttpGet]
    public async Task<IEnumerable<TReadDto>> GetAll()
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    [HttpGet]
    [Route("paging")]
    public async Task<PaginatedList<TReadDto>> GetAllPaged(int pageIndex, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TReadDto>>(result);
    }

    [HttpPost]
    public async Task<TReadDto> Create([FromBody] TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<TReadDto> Update(Guid id, [FromBody] TUpdateDto dto)
    {
        var entity = new TEntity {Id = id};
        DbContext.Set<TEntity>().Attach(entity);
        Mapper.Map(dto, entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<TReadDto> Delete(Guid id)
    {
        var entity = new TEntity {Id = id};
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

    [HttpDelete]
    public async Task<int> DeleteRange([FromBody] IEnumerable<Guid> ids)
    {
        var entities = ids.Select(id => new TEntity {Id = id}).ToList();
        DbContext.Set<TEntity>().AttachRange(entities);
        if (typeof(TEntity).IsAssignableTo(typeof(ISoftDelete)))
            entities.ForEach(entity => ((ISoftDelete) entity).Delete());
        else
            DbContext.Set<TEntity>().RemoveRange(entities);
        var result = await DbContext.SaveChangesAsync();
        return result;
    }

    #region Query Methods

    public async Task<IEnumerable<TReadDto>> QueryWhere(Expression<Func<TEntity, bool>> condition)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PaginatedList<TReadDto>> QueryWherePaged(Expression<Func<TEntity, bool>> condition, int pageIndex,
        int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TReadDto>>(result);
    }

    public async Task<IEnumerable<TReadDto>> Query(QueryOptions<TEntity> queryOptions)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PaginatedList<TReadDto>> QueryPaged(QueryOptions<TEntity> queryOptions, int pageIndex,
        int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToPagedListAsync(pageIndex, pageSize);
        return Mapper.Map<PaginatedList<TReadDto>>(result);
    }

    public async Task<TReadDto> QueryWhereSingle(Expression<Func<TEntity, bool>> condition)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .SingleOrDefaultAsync(condition);
        return Mapper.Map<TReadDto>(result);
    }

    public async Task<TReadDto> QuerySingle(QueryOptions<TEntity> queryOptions)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .FirstOrDefaultAsync();
        return Mapper.Map<TReadDto>(result);
    }

    #endregion
}

public class CrudService<TDbContext, TEntity, TReadDto, TCreateUpdateDto> :
    CrudService<TDbContext, TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}

public class CrudService<TDbContext, TEntity, TDto> :
    CrudService<TDbContext, TEntity, TDto, TDto, TDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}