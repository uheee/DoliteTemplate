namespace DoliteTemplate.Domain.Services.Base;

public interface ICreateService<in TCreateDto, TReadDto>
{
    Task<TReadDto> Create(TCreateDto dto);
}