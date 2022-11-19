using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services.Base;

namespace DoliteTemplate.Domain.Services;

public interface IDeviceService : ICrudService<Device, DeviceReadDto, DeviceCreateUpdateDto>
{
}