using DoliteTemplate.Api.Utils;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Domain.Utils;
using DoliteTemplate.Infrastructure.DbContexts;
using DoliteTemplate.Infrastructure.Utils;

namespace DoliteTemplate.Api.Services;

public class DeviceServiceImpl : BaseService<ApiDbContext>, IDeviceService
{
    public async Task<PagedList<DeviceReadDto>> GetExampleDevice(bool exception)
    {
        // var devices = await PagingQuery(context => context.Devices, 1, 10);
        var devices = await DbContext.Devices.ToPagedListAsync(1, 10);
        if (exception) throw ExceptionFactory.Business(10001, devices.Content.First().Name);
        return Mapper.Map<PagedList<DeviceReadDto>>(devices);
    }
}