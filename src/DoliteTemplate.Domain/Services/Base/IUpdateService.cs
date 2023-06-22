namespace DoliteTemplate.Domain.Services.Base;

public interface IUpdateService<in TKey, in TUpdateDto>
{
    Task<int> Update(TKey id, TUpdateDto dto);
}