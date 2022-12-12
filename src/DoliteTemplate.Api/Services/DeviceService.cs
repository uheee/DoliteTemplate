using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Services;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace DoliteTemplate.Api.Services;

/// <summary>
///     Device Service
/// </summary>
[ApiService(Rule = "Test.*")]
public class DeviceService :
    CrudService<ApiDbContext, Device, DeviceReadDto, DeviceCreateDto, DeviceUpdateDto>,
    IDeviceService
{
    /// <summary>
    ///     Test non-return
    /// </summary>
    [HttpGet]
    [Route("non-return")]
    public void TestNonReturn()
    {
    }

    /// <summary>
    ///     Test default arguments
    /// </summary>
    /// <param name="i">Leave it empty</param>
    /// <param name="s">Leave it empty</param>
    /// <param name="t">Leave it empty</param>
    /// <returns>Should be {100, "Hello", null}</returns>
    [HttpGet]
    [Route("default-arguments")]
    public object TestDefaultArguments(int i = 100, string s = "Hello", DateTime? t = null)
    {
        return new { i, s, t };
    }

    /// <summary>
    ///     Test exception
    /// </summary>
    /// <returns>Never</returns>
    /// <exception cref="BusinessException"></exception>
    [HttpGet]
    [Route("exception")]
    public string TestException()
    {
        throw ExceptionFactory.Business(10001);
    }
}