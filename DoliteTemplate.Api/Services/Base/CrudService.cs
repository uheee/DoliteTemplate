using System.Linq.Expressions;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class CrudService<TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto> :
    BaseService<TDbContext>,
    ICrudService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    public async Task<TReadDto> Get(Guid id)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .SingleAsync(entity => entity.Id == id);
        return Mapper.Map<TReadDto>(result);
    }

    public async Task<IEnumerable<TReadDto>> GetAll()
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<IEnumerable<TReadDto>> GetWhere(Expression<Func<TEntity, bool>> condition)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> GetAllPaged(int index, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> GetWherePaged(Expression<Func<TEntity, bool>> condition, int index,
        int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<IEnumerable<TReadDto>> Query(QueryOptions<TEntity> queryOptions)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> QueryPaged(QueryOptions<TEntity> queryOptions, int index, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<TReadDto> Create(TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

    public async Task<TReadDto> Update(Guid id, TUpdateDto dto)
    {
        var entity = new TEntity { Id = id };
        DbContext.Set<TEntity>().Attach(entity);
        Mapper.Map(dto, entity);
        await DbContext.SaveChangesAsync();
        return await Get(entity.Id);
    }

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

    public async Task<int> DeleteRange(IEnumerable<Guid> ids)
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