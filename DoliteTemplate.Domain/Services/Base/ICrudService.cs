using System.Linq.Expressions;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Domain.Services.Base;

public interface ICrudService<TEntity, TReadDto, in TCreateDto, in TUpdateDto>
    where TEntity : BaseEntity
{
    Task<TReadDto> Get(Guid id);
    Task<IEnumerable<TReadDto>> Get();
    Task<IEnumerable<TReadDto>> Get(Expression<Func<TEntity, bool>> condition);
    Task<PagedList<TReadDto>> Get(int index, int pageSize);
    Task<PagedList<TReadDto>> Get(Expression<Func<TEntity, bool>> condition, int index, int pageSize);
    Task<IEnumerable<TReadDto>> Get(QueryOptions<TEntity> queryOptions);
    Task<PagedList<TReadDto>> Get(QueryOptions<TEntity> queryOptions, int index, int pageSize);
    Task<TReadDto> Create(TCreateDto dto);
    Task<TReadDto> Update(Guid id, TUpdateDto dto);
    Task<TReadDto> Delete(Guid id);
    Task<int> Delete(IEnumerable<Guid> ids);
}

public interface ICrudService<TEntity, TReadDto, in TCreateUpdateDto> :
    ICrudService<TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>
    where TEntity : BaseEntity
{
}

public interface ICrudService<TEntity, TDto> :
    ICrudService<TEntity, TDto, TDto, TDto>
    where TEntity : BaseEntity
{
}