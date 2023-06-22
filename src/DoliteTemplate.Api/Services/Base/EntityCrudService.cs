using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto> :
    CrudService<TService, TDbContext, TEntity, Guid, TReadDto, TCreateDto, TUpdateDto>,
    IEntityCrudService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
    /// <summary>
    ///     Update an entity
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="dto">New entity DTO</param>
    /// <returns>The updated entity</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public override async Task<int> Update(Guid id, [FromBody] TUpdateDto dto)
    {
        var entity = new TEntity {Id = id};
        DbContext.Set<TEntity>().Attach(entity);
        Mapper.Map(dto, entity);
        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Delete an entity
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns>The deleted entity</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public override async Task<int> Delete(Guid id)
    {
        var entity = new TEntity {Id = id};
        DbContext.Set<TEntity>().Attach(entity);
        DbContext.Set<TEntity>().Remove(entity);
        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Delete some entities
    /// </summary>
    /// <param name="ids">Unique identifiers</param>
    /// <returns>Count of the deleted entities</returns>
    [HttpDelete]
    public override async Task<int> DeleteRange([FromBody] IEnumerable<Guid> ids)
    {
        var entities = ids.Select(id => new TEntity {Id = id}).ToList();
        DbContext.Set<TEntity>().AttachRange(entities);
        DbContext.Set<TEntity>().RemoveRange(entities);
        return await DbContext.SaveChangesAsync();
    }
}

public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto> :
    EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>,
    IEntityCrudService<TEntity, TReadDto, TCreateUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}

public class EntityCrudService<TService, TDbContext, TEntity, TDto> :
    EntityCrudService<TService, TDbContext, TEntity, TDto, TDto, TDto>,
    IEntityCrudService<TEntity, TDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new()
{
}