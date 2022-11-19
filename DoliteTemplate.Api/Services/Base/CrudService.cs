using System.Linq.Expressions;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class CrudService<TDbContext, TEntity, TReadDto, TCreateUpdateDto> :
    BaseService<TDbContext>,
    ICrudService<TEntity, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    public async Task<TReadDto> Read(Guid id)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .SingleAsync(entity => entity.Id == id);
        return Mapper.Map<TReadDto>(result);
    }

    public async Task<IEnumerable<TReadDto>> Read()
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<IEnumerable<TReadDto>> Read(Expression<Func<TEntity, bool>> condition)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> Read(int index, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> Read(Expression<Func<TEntity, bool>> condition, int index, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted()
            .Where(condition).ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<IEnumerable<TReadDto>> Read(QueryOptions<TEntity> queryOptions)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<TReadDto>>(result);
    }

    public async Task<PagedList<TReadDto>> Read(QueryOptions<TEntity> queryOptions, int index, int pageSize)
    {
        var result = await DbContext.Set<TEntity>().SkipDeleted().QueryBy(queryOptions)
            .ToPagedListAsync(index, pageSize);
        return Mapper.Map<PagedList<TReadDto>>(result);
    }

    public async Task<TReadDto> Create(TCreateUpdateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return await Read(entity.Id);
    }

    public async Task<TReadDto> Update(Guid id, TCreateUpdateDto dto)
    {
        var entity = new TEntity {Id = id};
        DbContext.Set<TEntity>().Attach(entity);
        Mapper.Map(dto, entity);
        await DbContext.SaveChangesAsync();
        return await Read(entity.Id);
    }

    public async Task<TReadDto> Delete(Guid id)
    {
        var entity = new TEntity {Id = id};
        DbContext.Set<TEntity>().Attach(entity);
        TEntity result;
        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.IsDeleted = true;
                result = entity;
                break;
            default:
                result = DbContext.Set<TEntity>().Remove(entity).Entity;
                break;
        }

        await DbContext.SaveChangesAsync();
        return Mapper.Map<TReadDto>(result);
    }

    public async Task<int> Delete(IEnumerable<Guid> ids)
    {
        var entities = ids.Select(id => new TEntity {Id = id}).ToList();
        DbContext.Set<TEntity>().AttachRange(entities);
        if (typeof(TEntity).IsAssignableTo(typeof(ISoftDelete)))
            entities.ForEach(entity => ((ISoftDelete) entity).IsDeleted = true);
        else
            DbContext.Set<TEntity>().RemoveRange(entities);
        var result = await DbContext.SaveChangesAsync();
        return result;
    }
}