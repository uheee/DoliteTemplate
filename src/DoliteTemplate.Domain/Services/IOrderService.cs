using DoliteTemplate.Domain.DTOs.Order;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services.Base;

namespace DoliteTemplate.Domain.Services;

public interface IOrderService : IEntityCrudService<Order, OrderReadDto, OrderCreateDto, OrderUpdateDto>
{
}