using Domain.DTOs;
using Domain.Services;
using Domain.Utils;
using Infrastructure.Utils;
using WebApi.Utils;

namespace WebApi.Services;

public class DeviceServiceImpl : BaseService, IDeviceService
{
    public async Task<PagedList<DeviceReadDto>> GetExampleDevice(bool exception)
    {
        // var devices = await PagingQuery(context => context.Devices, 1, 10);
        var devices = await DbContext.Devices.ToPagedListAsync(1, 10);
        if (exception) throw ExceptionFactory.Business(10001, devices.Content.First().Name);
        return Mapper.Map<PagedList<DeviceReadDto>>(devices);
    }
}