using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils.RestApi;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;

namespace DoliteTemplate.Api.Services;

[GenerateAPI]
public class DeviceService :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateDto, DeviceUpdateDto>,
    IDeviceService
{
}