using System.Data.Common;
using Autofac;
using DoliteTemplate.Infrastructure.DbContexts;
using Npgsql;

namespace DoliteTemplate.Api.Utils.Autofac;

public class DbModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var connectionString = GlobalDefinitions.Configuration.GetConnectionString(GlobalDefinitions.ConnectionString);
        if (connectionString is null)
            throw new Exception($"missing connection string '{GlobalDefinitions.ConnectionString}'");
        builder.Register(_ => new ApiDbContext(connectionString)).AsSelf();
        builder.Register(_ => NpgsqlDataSource.Create(connectionString).OpenConnection()).As<DbConnection>()
            .InstancePerLifetimeScope();
        builder.Register(context => context.Resolve<DbConnection>().BeginTransaction()).As<DbTransaction>();
        builder.RegisterGeneric(typeof(DbContextProvider<>)).AsSelf().PropertiesAutowired();
    }
}