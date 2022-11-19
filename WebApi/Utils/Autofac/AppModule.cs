using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using WebApi.Utils.Error;
using Module = Autofac.Module;

namespace WebApi.Utils.Autofac;

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