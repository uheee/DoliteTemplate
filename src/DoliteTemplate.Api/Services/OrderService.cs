using AutoMapper;
using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Domain.DTOs.Order;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.Extensions.Localization;
using Npgsql;
using StackExchange.Redis;
using Order = DoliteTemplate.Domain.Entities.Order;

namespace DoliteTemplate.Api.Services;

/// <summary>
///     Device Service
/// </summary>
[ApiService]
public class OrderService(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<OrderService> localizer,
    NpgsqlConnection dbConnection,
    Lazy<ApiDbContext> dbContextLazier,
    Lazy<DbContextProvider<ApiDbContext>> dbContextProviderLazier) :
    EntityCrudService<OrderService, ApiDbContext, Order, OrderReadDto, OrderCreateDto, OrderUpdateDto>(
        mapper,
        redis,
        httpContextAccessor,
        encryptHelper,
        localizer,
        dbConnection,
        dbContextLazier,
        dbContextProviderLazier),
    IOrderService;