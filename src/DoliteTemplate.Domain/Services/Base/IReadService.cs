using DoliteTemplate.Shared.Utils;

namespace DoliteTemplate.Domain.Services.Base;

public interface IReadService<in TKey, TReadDto>
{
    Task<TReadDto?> Get(TKey id);
    Task<IEnumerable<TReadDto>> GetAll();
    Task<PaginatedList<TReadDto>> GetAllPaged(int pageIndex, int pageSize);
}