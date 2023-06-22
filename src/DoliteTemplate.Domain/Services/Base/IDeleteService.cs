namespace DoliteTemplate.Domain.Services.Base;

public interface IDeleteService<in TKey>
{
    Task<int> Delete(TKey id);
    Task<int> DeleteRange(IEnumerable<TKey> ids);
}