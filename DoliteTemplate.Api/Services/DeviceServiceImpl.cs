using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.DbContexts;
using DoliteTemplate.Infrastructure.Utils;

namespace DoliteTemplate.Api.Services;

public class DeviceServiceImpl :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateUpdateDto>,
    IDeviceService
{
    public async Task<PagedList<DeviceReadDto>> GetPagingDevices()
    {
        // var devices = await PagingQuery(context => context.Devices, 1, 10);
        var devices = await DbContext.Devices.ToPagedListAsync(1, 10);
        return Mapper.Map<PagedList<DeviceReadDto>>(devices);
    }

    public async Task CreateDevice(DeviceCreateUpdateDto dto)
    {
        var device = Mapper.Map<Device>(dto);
        DbContext.Devices.Add(device);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateDevice(Guid id, DeviceCreateUpdateDto dto)
    {
        var device = new Device {Id = id};
        DbContext.Devices.Attach(device);
        Mapper.Map(dto, device);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteDevice(Guid id)
    {
        var device = new Device {Id = id};
        DbContext.Devices.Attach(device);
        DbContext.Devices.Remove(device);
        await DbContext.SaveChangesAsync();
    }
}