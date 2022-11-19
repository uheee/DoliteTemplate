using System.Reflection;
using Autofac;
using DoliteTemplate.Api.Utils.Error;
using Microsoft.AspNetCore.Mvc;
using Module = Autofac.Module;

namespace DoliteTemplate.Api.Utils.Autofac;

public class AppModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.BaseType == typeof(ControllerBase))
            .PropertiesAutowired();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.Name.EndsWith("Impl"))
            .AsImplementedInterfaces().PropertiesAutowired();
        builder.RegisterType<ExceptionFactory>().PropertiesAutowired();
        // Set custom injection rules
    }
}