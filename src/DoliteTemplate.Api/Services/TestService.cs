using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DoliteTemplate.Api.Services;

[ApiService]
public class TestService : BaseService<TestService>
{
    [HttpGet]
    public int GetZero()
    {
        return 0;
    }
}