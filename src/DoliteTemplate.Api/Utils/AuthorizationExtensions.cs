using System.Reflection;
using System.Text.RegularExpressions;
using DoliteTemplate.Shared.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DoliteTemplate.Api.Utils;

public static partial class AuthorizationExtensions
{
    private static readonly Regex RoleRegex = GenerateRoleRegex();

    public static void AutoSetPolicies(this AuthorizationOptions options)
    {
        var policies = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(type => type.BaseType == typeof(ControllerBase))
            .SelectMany(type =>
            {
                var typeAttribute = type.GetCustomAttribute<AuthorizeAttribute>();
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var methodAttributes = methods.Select(method => method.GetCustomAttribute<AuthorizeAttribute>())
                    .Where(attribute => attribute is not null).ToList();
                if (typeAttribute is not null)
                {
                    methodAttributes.Add(typeAttribute);
                }

                return methodAttributes;
            })
            .Select(attribute => attribute!.Policy);
        foreach (var policy in policies)
        {
            if (string.IsNullOrEmpty(policy))
            {
                continue;
            }

            var match = RoleRegex.Match(policy);
            if (!match.Success)
            {
                continue;
            }

            var requiredRoles = match.Groups[1].Value.Split(new[] { ' ', ',', '|' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var allowedRoles = requiredRoles.SelectMany(GetRoleParts).Distinct();
            options.AddPolicy(policy, config => config.AddRequirements(
                new ClaimsAuthorizationRequirement(ClaimKeys.Role, allowedRoles)));
        }
    }

    private static IEnumerable<string> GetRoleParts(string role)
    {
        var parts = role.Split('.');
        for (var i = 0; i < parts.Length; i++)
        {
            yield return string.Join('.', parts[..(i + 1)]);
        }
    }

    [GeneratedRegex(@"role[s]?\s*\(\s*([\w\-\.\s\,\|]*)\s*\)")]
    private static partial Regex GenerateRoleRegex();

    public static void ConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration, ECDsaSecurityKey securityKey)
    {
        var schema = configuration["Authentication:Schema"];
        if (string.IsNullOrEmpty(schema))
        {
            return;
        }

        services.AddAuthentication(schema)
            .AddScheme<JwtBearerOptions, JwtBearerExclusiveLoginHandler>(schema, options =>
            {
                var tokenValidationParameters = configuration
                    .GetSection("Authentication:TokenValidationParameters")
                    .Get<TokenValidationParameters>();
                if (tokenValidationParameters is null)
                {
                    return;
                }

                tokenValidationParameters.IssuerSigningKey = securityKey;
                options.TokenValidationParameters = tokenValidationParameters;
            });
    }
}