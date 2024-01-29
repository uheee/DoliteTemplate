using System.Reflection;
using System.Text.RegularExpressions;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     认证授权扩展
/// </summary>
public static partial class AuthExtensions
{
    private static readonly Regex RoleRegex = GenerateRoleRegex();

    /// <summary>
    ///     配置授权模式
    ///     <remarks>该方法自动为应用配置以Service类或方法上指定的<see cref="AuthorizeAttribute" />为配置项的授权模式</remarks>
    ///     <example>如要配置某个Service类或方法仅授权给administer角色，需要配置为<b>[Authorize("role(administrator)")]</b></example>
    /// </summary>
    /// <param name="options">认证授权配置</param>
    /// <param name="assembly">当前程序集</param>
    public static void AutoSetPolicies(this AuthorizationOptions options, Assembly assembly)
    {
        var policies = assembly.GetExportedTypes()
            .Where(type => type.IsAssignableTo(typeof(ControllerBase)))
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

            var requiredRoles = match.Groups[1].Value.Split(Strings.Separator,
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

    /// <summary>
    ///     配置认证模式
    ///     <remarks>该方法自动为应用配置基于ApiKey JWT token的认证模式</remarks>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="securityKey">密钥</param>
    public static void ConfigureAuthentication(this IServiceCollection services, ECDsaSecurityKey securityKey)
    {
        #region Bearer

        services.AddAuthentication("Bearer")
            .AddScheme<JwtBearerOptions, JwtBearerExclusiveLoginHandler>("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidTypes = ["JWT"],
                    ValidAlgorithms = ["ES256"],
                    IssuerSigningKey = securityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true
                };
            });

        #endregion
    }
}