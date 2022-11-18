using System.Reflection;
using Autofac;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApi.Utils.Error;
using WebApi.Utils.Localization;
using Module = Autofac.Module;

namespace WebApi.Utils.Autofac;

public class BaseModule : Module
{
    private readonly IConfiguration _configuration;

    public BaseModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.BaseType == typeof(ControllerBase))
            .PropertiesAutowired();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.Name.EndsWith("Impl"))
            .AsImplementedInterfaces().PropertiesAutowired();
        builder.RegisterInstance(new Resource(GlobalDefinitions.LocalizationPath));
        builder.RegisterType<ExceptionFactory>().PropertiesAutowired();
        builder.RegisterInstance(new DbContextOptionsBuilder<ApiDbContext>()
            .UseNpgsql(new NpgsqlConnection(_configuration.GetConnectionString(GlobalDefinitions.ConnectionString)))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).SingleInstance();
        // Set custom injection rules
    }
}