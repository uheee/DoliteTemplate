using AutoMapper;
using DoliteTemplate.Domain.DTOs.Order;
using DoliteTemplate.Domain.Entities;

namespace DoliteTemplate.Api.Utils.AutoMapper;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderReadDto>();
        CreateMap<OrderCreateDto, Order>();
        CreateMap<OrderUpdateDto, Order>()
            .OnlyForNotNullProperties();
    }
}