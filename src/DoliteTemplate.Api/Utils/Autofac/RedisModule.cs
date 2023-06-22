using Autofac;
using DoliteTemplate.Shared.Constants;
using StackExchange.Redis;

namespace DoliteTemplate.Api.Utils.Autofac;

public class RedisModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var connectionString = configuration.GetConnectionString(ConnectionStrings.Redis);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"missing '{ConnectionStrings.Redis}' connection string");
            }

            return ConnectionMultiplexer.Connect(connectionString);
        }).AsImplementedInterfaces().SingleInstance();
    }
}