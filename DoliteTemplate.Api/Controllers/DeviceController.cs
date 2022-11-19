using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DoliteTemplate.Api.Controllers;

/// <summary>
///     Greeting
/// </summary>
[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    // public IDeviceService DeviceService { get; init; } = null!;
    public ICrudService<Device, DeviceReadDto, DeviceCreateDto, DeviceUpdateDto> DeviceService { get; init; } = null!;

    /// <summary>
    ///     Get Paging Devices
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<DeviceReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetPagingDevices(int index = 1, int pageSize = 10)
    {
        var result = await DeviceService.Get(index, pageSize);
        return Ok(result);
    }

    /// <summary>
    ///     Create Device
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateDevice(DeviceCreateDto body)
    {
        await DeviceService.Create(body);
        return Ok(true);
    }

    /// <summary>
    ///     Update Device
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateDevice(Guid id, DeviceUpdateDto body)
    {
        await DeviceService.Update(id, body);
        return Ok(true);
    }

    /// <summary>
    ///     Delete Device
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateDevice(Guid id)
    {
        await DeviceService.Delete(id);
        return Ok(true);
    }
}