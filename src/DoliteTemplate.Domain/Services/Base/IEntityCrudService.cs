namespace DoliteTemplate.Domain.Services.Base;

public interface IEntityCrudService<TEntity, TReadDto, in TCreateDto, in TUpdateDto> :
    ICrudService<TEntity, Guid, TReadDto, TCreateDto, TUpdateDto>
{
}

public interface IEntityCrudService<TEntity, TReadDto, in TCreateUpdateDto> :
    IEntityCrudService<TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>
{
}

public interface IEntityCrudService<TEntity, TDto> :
    IEntityCrudService<TEntity, TDto, TDto, TDto>
{
}