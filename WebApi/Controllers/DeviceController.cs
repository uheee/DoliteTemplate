using Domain.DTOs;
using Domain.Services;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using WebApi.Utils.Error;

namespace WebApi.Controllers;

/// <summary>
///     Greeting
/// </summary>
[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    public IDeviceService DeviceService { get; init; } = null!;

    /// <summary>
    ///     Get Example Device
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("example")]
    [ProducesResponseType(typeof(PagedList<DeviceReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetExampleDevice(bool exception = false)
    {
        var result = await DeviceService.GetExampleDevice(exception);
        return Ok(result);
    }
}