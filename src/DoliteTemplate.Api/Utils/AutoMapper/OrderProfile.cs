using AutoMapper;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;

namespace DoliteTemplate.Api.Utils.AutoMapper;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        this.CreateCrudMap<Order, OrderReadDto, OrderCreateDto, OrderUpdateDto>();
    }
}