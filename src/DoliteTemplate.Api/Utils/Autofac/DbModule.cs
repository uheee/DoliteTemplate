using System.Data.Common;
using Autofac;
using DoliteTemplate.Infrastructure.DbContexts;
using DoliteTemplate.Shared.Constants;
using Npgsql;

namespace DoliteTemplate.Api.Utils.Autofac;

public class DbModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(context =>
                new ApiDbContext(context.Resolve<IConfiguration>().GetConnectionString(ConnectionStrings.Database)!))
            .AsSelf().PropertiesAutowired();
        builder.Register(context =>
                NpgsqlDataSource
                    .Create(context.Resolve<IConfiguration>().GetConnectionString(ConnectionStrings.Database)!)
                    .OpenConnection())
            .AsSelf().As<DbConnection>().InstancePerLifetimeScope();
        builder.Register(context =>
                new NpgsqlLargeObjectManager(context.Resolve<NpgsqlConnection>()))
            .AsSelf();
        builder.Register(context => context.Resolve<DbConnection>().BeginTransaction()).As<DbTransaction>();
        builder.RegisterGeneric(typeof(DbContextProvider<>)).AsSelf().PropertiesAutowired();
    }
}