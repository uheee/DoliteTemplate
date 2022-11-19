using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Domain.Services;

public interface IDeviceService
{
    Task<PagedList<DeviceReadDto>> GetExampleDevice(bool exception);
}