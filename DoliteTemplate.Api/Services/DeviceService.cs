using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;

namespace DoliteTemplate.Api.Services;

public class DeviceService :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateUpdateDto>,
    IDeviceService
{
}