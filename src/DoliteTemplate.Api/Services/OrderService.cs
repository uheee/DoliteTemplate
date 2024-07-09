using Asp.Versioning;
using DoliteTemplate.Api.Shared.Services;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Order = DoliteTemplate.Domain.Entities.Order;

namespace DoliteTemplate.Api.Services;

/// <summary>
///     Device Service
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("[controller]")]
public class OrderService : EntityCrudService<
    OrderService,
    ApiDbContext,
    Order,
    OrderReadDto,
    OrderCreateDto,
    OrderUpdateDto>;