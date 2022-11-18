using Domain.DTOs;
using Domain.Utils;

namespace Domain.Services;

public interface IDeviceService
{
    Task<PagedList<DeviceReadDto>> GetExampleDevice(bool exception);
}