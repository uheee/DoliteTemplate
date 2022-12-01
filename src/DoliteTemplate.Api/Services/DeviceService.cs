using System.ComponentModel;
using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Api.Utils.RestApi;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace DoliteTemplate.Api.Services;

[GenerateAPI]
public class DeviceService :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateDto, DeviceUpdateDto>,
    IDeviceService
{
    
}