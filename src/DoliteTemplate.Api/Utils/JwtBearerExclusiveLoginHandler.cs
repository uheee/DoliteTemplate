using System.Security.Claims;
using System.Text.Encodings.Web;
using DoliteTemplate.Shared.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DoliteTemplate.Api.Utils;

public class JwtBearerExclusiveLoginHandler : JwtBearerHandler
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConnectionMultiplexer _redis;
    private bool _expiredFlag;

    public JwtBearerExclusiveLoginHandler(
        IConnectionMultiplexer redis,
        IWebHostEnvironment environment,
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) :
        base(options, logger, encoder, clock)
    {
        _redis = redis;
        _environment = environment;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();
        if (_environment.IsDevelopment() || !result.Succeeded)
        {
            return result;
        }

        var currentToken = Request.Headers.Authorization.ToString()[("Bearer".Length + 1)..];
        var userId = result.Ticket.Principal.FindFirstValue(ClaimKeys.UserId);
        var key = $"user:token:{userId}";
        string? cachedToken = await _redis.GetDatabase().StringGetAsync(key);
        if (cachedToken is null || string.Equals(currentToken, cachedToken))
        {
            return AuthenticateResult.Success(result.Ticket);
        }

        _expiredFlag = true;
        return AuthenticateResult.Fail("expired_token");
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await base.HandleChallengeAsync(properties);
        if (_expiredFlag)
        {
            Response.Headers.WWWAuthenticate =
                Response.Headers.WWWAuthenticate.ToString().Replace("invalid_token", "expired_token");
        }
    }
}