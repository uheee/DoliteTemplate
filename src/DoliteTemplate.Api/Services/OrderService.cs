using AutoMapper;
using DoliteTemplate.Api.Shared.Services;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Order = DoliteTemplate.Domain.Entities.Order;

namespace DoliteTemplate.Api.Services;

/// <summary>
///     Device Service
/// </summary>
[ApiController]
[Route("[controller]")]
public class OrderService(
    IMapper mapper,
    IStringLocalizer<OrderService> localizer,
    Lazy<ApiDbContext> dbContextProvider) :
    EntityCrudService<OrderService, ApiDbContext, Order, OrderReadDto, OrderCreateDto, OrderUpdateDto>(
        mapper,
        localizer,
        dbContextProvider);