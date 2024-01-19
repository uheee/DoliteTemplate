using System.Net.Mime;
using DoliteTemplate.Api.Shared.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     异常处理扩展
/// </summary>
public static class ExceptionHandlerExtensions
{
    /// <summary>
    ///     捕获全局异常并处理
    ///     <remarks>处理方式：</remarks>
    ///     <remarks>当异常类型为<see cref="BusinessException" />或<see cref="DuplicateException" />时，返回HTTP400错误；</remarks>
    ///     <remarks>否则返回HTTP500错误。</remarks>
    /// </summary>
    /// <param name="app">当前Web应用</param>
    public static void UseExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
                var error = exception.ToErrorInfo(app);
                context.Response.ContentType = MediaTypeNames.Application.Json;
                context.Response.StatusCode = exception switch
                {
                    (BusinessException or DuplicateException) => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                var jsonOptions = app.Services.GetService<IOptions<JsonOptions>>()!.Value;
                await context.Response.WriteAsJsonAsync(error, jsonOptions.JsonSerializerOptions);
            });
        });
    }
}