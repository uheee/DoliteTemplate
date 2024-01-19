using System.Reflection;
using Autofac;
using DoliteTemplate.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Module = Autofac.Module;

namespace DoliteTemplate.Api.Utils.Autofac;

/// <summary>
///     应用映射模块
/// </summary>
public class AppModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.BaseType == typeof(ControllerBase))
            .PropertiesAutowired();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(type => type.IsAssignableTo<BaseService>())
            .AsImplementedInterfaces()
            .AsSelf()
            .PropertiesAutowired();
    }
}