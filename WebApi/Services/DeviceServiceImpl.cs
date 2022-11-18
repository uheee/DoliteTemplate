using Domain.DTOs;
using Domain.Entities;
using Domain.Services;
using Domain.Utils;
using Infrastructure;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using WebApi.Utils;

namespace WebApi.Services;

public class DeviceServiceImpl : BaseService, IDeviceService
{
    public async Task<PagedList<DeviceReadDto>> GetExampleDevice(bool exception)
    {
        var devices = await DbContext.Devices.ToPagedListAsync(1, 10);
        if (exception) throw ExceptionFactory.Business(10001, devices.Content.First().Name);
        return Mapper.Map<PagedList<DeviceReadDto>>(devices);
    }
}