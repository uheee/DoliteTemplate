using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoliteTemplate.Infrastructure.Utils;

/// <summary>
///     模型构建扩展
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     对某类型的实体执行操作
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    /// <param name="buildAction">操作</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>模型构建器</returns>
    public static ModelBuilder EntitiesOfType<TEntity>(this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder> buildAction) where TEntity : class
    {
        return modelBuilder.EntitiesOfType(typeof(TEntity), buildAction);
    }

    /// <summary>
    ///     对某类型的实体执行操作
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    /// <param name="type">实体类型</param>
    /// <param name="buildAction">操作</param>
    /// <returns>模型构建器</returns>
    public static ModelBuilder EntitiesOfType(this ModelBuilder modelBuilder, Type type,
        Action<EntityTypeBuilder> buildAction)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (type.IsAssignableFrom(entityType.ClrType))
            {
                buildAction(modelBuilder.Entity(entityType.ClrType));
            }
        }

        return modelBuilder;
    }
}