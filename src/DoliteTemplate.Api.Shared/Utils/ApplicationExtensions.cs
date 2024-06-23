using System.Data.Common;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;

namespace DoliteTemplate.Api.Shared.Utils;

public static class ApplicationExtensions
{
    public static void ConfigureDbContext<TDbContext>(
        this IHostApplicationBuilder builder,
        Func<string, DbDataSource> dataSourceBuilder,
        Action<DbContextOptionsBuilder, DbDataSource> optionsBuilder,
        bool throwsWhenEmpty = false) where TDbContext : DbContext
    {
        var name = typeof(TDbContext).GetComponentName(typeof(DbContext));
        var connectionString = builder.Configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (!throwsWhenEmpty)
            {
                return;
            }

            throw new Exception($"Missing '{name}' in ConnectionStrings");
        }

        var dataSource = dataSourceBuilder(connectionString);
        builder.Services.AddDbContext<TDbContext>(options =>
        {
            optionsBuilder(options, dataSource);
        });
    }

    public static void ConfigureDbContextFactory<TDbContext>(
        this IHostApplicationBuilder builder,
        Func<string, DbDataSource> dataSourceBuilder,
        Action<DbContextOptionsBuilder, DbDataSource> optionsBuilder,
        bool throwsWhenEmpty = false) where TDbContext : DbContext
    {
        var name = typeof(TDbContext).GetComponentName(typeof(DbContext));
        var connectionString = builder.Configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (!throwsWhenEmpty)
            {
                return;
            }

            throw new Exception($"Missing '{name}' in ConnectionStrings");
        }

        var dataSource = dataSourceBuilder(connectionString);
        builder.Services.AddDbContextFactory<TDbContext>(options =>
        {
            optionsBuilder(options, dataSource);
        });
    }

    public static void ConfigureRedis(this IHostApplicationBuilder builder, string connectionStringTag)
    {
        builder.Services.AddSingleton<ISerializer>(
            new SystemTextJsonSerializer(SerializerExtensions.JsonSerializerOptions));
        builder.Services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(new RedisConfiguration
        {
            ConnectionString = builder.Configuration.GetConnectionString(connectionStringTag)
        });
    }

    public static void ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
            .CreateLogger());
    }

    public static void ConfigureAutofac(this IHostApplicationBuilder builder,
        Action<ContainerBuilder>? buildAction = null, params Assembly[] assemblies)
    {
        builder.ConfigureContainer(new AutofacServiceProviderFactory(), cb =>
        {
            cb.RegisterAssemblyModules(assemblies);
            buildAction?.Invoke(cb);
        });
    }

    public static void ConfigureAutoMapper(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        builder.Services.AddAutoMapper(assemblies);
    }
}