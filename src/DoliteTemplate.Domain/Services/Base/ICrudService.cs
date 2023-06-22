namespace DoliteTemplate.Domain.Services.Base;

public interface ICrudService<TEntity, in TKey, TReadDto, in TCreateDto, in TUpdateDto> :
    IReadService<TKey, TReadDto>,
    ICreateService<TCreateDto, TReadDto>,
    IUpdateService<TKey, TUpdateDto>,
    IDeleteService<TKey>
{
}

public interface ICrudService<TEntity, in TKey, TReadDto, in TCreateUpdateDto> :
    ICrudService<TEntity, TKey, TReadDto, TCreateUpdateDto, TCreateUpdateDto>
{
}

public interface ICrudService<TEntity, in TKey, TDto> :
    ICrudService<TEntity, TKey, TDto, TDto, TDto>
{
}