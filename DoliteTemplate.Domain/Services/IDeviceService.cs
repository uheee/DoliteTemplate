using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Domain.Services;

public interface IDeviceService : ICrudService<Device, DeviceReadDto, DeviceCreateUpdateDto>
{
    Task<PagedList<DeviceReadDto>> GetPagingDevices();
    Task CreateDevice(DeviceCreateUpdateDto dto);
    Task UpdateDevice(Guid id, DeviceCreateUpdateDto dto);
    Task DeleteDevice(Guid id);
}