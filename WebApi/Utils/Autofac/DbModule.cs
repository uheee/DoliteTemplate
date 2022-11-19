using System.Data.Common;
using Autofac;
using Infrastructure.DbContexts;
using Npgsql;

namespace WebApi.Utils.Autofac;

public class DbModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var connectionString = GlobalDefinitions.Configuration.GetConnectionString(GlobalDefinitions.ConnectionString);
        if (connectionString is null)
            throw new Exception($"missing connection string '{GlobalDefinitions.ConnectionString}'");
        var dataSource = NpgsqlDataSource.Create(connectionString);
        builder.RegisterInstance(dataSource).As<DbDataSource>().SingleInstance();
        builder.Register(context => new ApiDbContext(context.Resolve<DbDataSource>())).AsSelf();
        builder.Register(context => context.Resolve<DbDataSource>().OpenConnection()).As<DbConnection>()
            .InstancePerLifetimeScope();
        builder.Register(context => context.Resolve<DbConnection>().BeginTransaction()).As<DbTransaction>();
        builder.RegisterType<TransactionDbContextProvider>().AsSelf().PropertiesAutowired();
    }
}