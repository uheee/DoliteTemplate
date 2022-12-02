using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;

namespace DoliteTemplate.Api.Services;

/// <summary>
///     Device Service
/// </summary>
[ApiService]
public class DeviceService :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateDto, DeviceUpdateDto>,
    IDeviceService
{
}