using AutoMapper;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Npgsql;
using StackExchange.Redis;

namespace DoliteTemplate.Api.Services.Base;

public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateDto, TUpdateDto>(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<TService> localizer,
    NpgsqlConnection dbConnection,
    Lazy<TDbContext> dbContextLazier,
    Lazy<DbContextProvider<TDbContext>> dbContextProviderLazier) :
    CrudService<TService, TDbContext, TEntity, Guid, TReadDto, TCreateDto, TUpdateDto>(
        mapper,
        redis,
        httpContextAccessor,
        encryptHelper,
        localizer,
        dbConnection,
        dbContextLazier,
        dbContextProviderLazier),
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
    var query = DbContext.Set<TEntity>().AsTracking();
    query = AfterQuery(query);
    var entity = await query.SingleOrDefaultAsync(entity => entity.Id == id);
    if (entity is null)
    {
        return 0;
    }

    mapper.Map(dto, entity);
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
    var entity = new TEntity { Id = id };
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
    var entities = ids.Select(id => new TEntity { Id = id }).ToList();
    DbContext.Set<TEntity>().AttachRange(entities);
    DbContext.Set<TEntity>().RemoveRange(entities);
    return await DbContext.SaveChangesAsync();
}
}

public class EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<TService> localizer,
    NpgsqlConnection dbConnection,
    Lazy<TDbContext> dbContextLazier,
    Lazy<DbContextProvider<TDbContext>> dbContextProviderLazier) :
    EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>(
        mapper,
        redis,
        httpContextAccessor,
        encryptHelper,
        localizer,
        dbConnection,
        dbContextLazier,
        dbContextProviderLazier),
    IEntityCrudService<TEntity, TReadDto, TCreateUpdateDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TReadDto, TCreateUpdateDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new();

public class EntityCrudService<TService, TDbContext, TEntity, TDto>(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<TService> localizer,
    NpgsqlConnection dbConnection,
    Lazy<TDbContext> dbContextLazier,
    Lazy<DbContextProvider<TDbContext>> dbContextProviderLazier) :
    EntityCrudService<TService, TDbContext, TEntity, TDto, TDto, TDto>(
        mapper,
        redis,
        httpContextAccessor,
        encryptHelper,
        localizer,
        dbConnection,
        dbContextLazier,
        dbContextProviderLazier),
    IEntityCrudService<TEntity, TDto>
    where TService : EntityCrudService<TService, TDbContext, TEntity, TDto>
    where TDbContext : DbContext
    where TEntity : BaseEntity, new();