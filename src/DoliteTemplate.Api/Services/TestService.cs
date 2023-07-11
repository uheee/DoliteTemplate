using DoliteTemplate.Api.Services.Base;
using DoliteTemplate.Api.Utils;
using GraphQL.AspNet.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace DoliteTemplate.Api.Services;

[ApiService]
public class TestService : BaseService<TestService>
{
    [Query]
    public int GetZero()
    {
        return 0;
    }
}