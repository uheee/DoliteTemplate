using System.Linq.Expressions;
using DoliteTemplate.Domain.Base;

namespace DoliteTemplate.Domain.Utils;

public interface ICrudService<TEntity, TReadDto, in TCreateUpdateDto>
    where TEntity : BaseEntity
{
    Task<TReadDto> Read(Guid id);
    Task<IEnumerable<TReadDto>> Read();
    Task<IEnumerable<TReadDto>> Read(Expression<Func<TEntity, bool>> condition);
    Task<PagedList<TReadDto>> Read(int index, int pageSize);
    Task<PagedList<TReadDto>> Read(Expression<Func<TEntity, bool>> condition, int index, int pageSize);
    Task<IEnumerable<TReadDto>> Read(QueryOptions<TEntity> queryOptions);
    Task<PagedList<TReadDto>> Read(QueryOptions<TEntity> queryOptions, int index, int pageSize);
    Task<TReadDto> Create(TCreateUpdateDto dto);
    Task<TReadDto> Update(Guid id, TCreateUpdateDto dto);
    Task<TReadDto> Delete(Guid id);
    Task<int> Delete(IEnumerable<Guid> ids);
}