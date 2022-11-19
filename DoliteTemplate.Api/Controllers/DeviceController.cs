using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Services;
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
    public IDeviceService DeviceService { get; init; } = null!;

    /// <summary>
    ///     Get Paging Devices
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<DeviceReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetPagingDevices(int index, int pageSize)
    {
        var result = await DeviceService.Read(index, pageSize);
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
    public async Task<ActionResult> CreateDevice(DeviceCreateUpdateDto body)
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
    public async Task<ActionResult> UpdateDevice(Guid id, DeviceCreateUpdateDto body)
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