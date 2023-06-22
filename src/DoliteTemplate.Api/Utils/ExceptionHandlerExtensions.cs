using System.Net.Mime;
using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DoliteTemplate.Api.Utils;

public static class ExceptionHandlerExtensions
{
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