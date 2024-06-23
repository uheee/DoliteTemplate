using System.Reflection;
using Autofac;
using DoliteTemplate.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Module = Autofac.Module;

namespace DoliteTemplate.Api.Shared.Utils.Autofac;

/// <summary>
///     应用映射模块
/// </summary>
public class AppModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = Assembly.GetEntryAssembly()!;
        builder.RegisterAssemblyTypes(assembly)
            .Where(type => type.BaseType == typeof(ControllerBase))
            .AsImplementedInterfaces()
            .AsSelf()
            .PropertiesAutowired();
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(BaseService<>))
            .AsImplementedInterfaces()
            .AsSelf()
            .PropertiesAutowired();
    }
}