using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Domain.Services.Base;

public interface ICrudService<TEntity, TReadDto, in TCreateDto, in TUpdateDto>
    where TEntity : BaseEntity
{
    Task<TReadDto> Get(Guid id);
    Task<IEnumerable<TReadDto>> GetAll();
    Task<PaginatedList<TReadDto>> GetAllPaged(int pageIndex, int pageSize);
    Task<TReadDto> Create(TCreateDto dto);
    Task<TReadDto> Update(Guid id, TUpdateDto dto);
    Task<TReadDto> Delete(Guid id);
    Task<int> DeleteRange(IEnumerable<Guid> ids);
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