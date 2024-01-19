using System.Linq.Expressions;
using System.Reflection;
using CaseExtensions;
using DoliteTemplate.Api.Shared.Errors;
using DoliteTemplate.Api.Shared.Utils;
using DoliteTemplate.Domain.Shared.Entities;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DoliteTemplate.Infrastructure.DbContexts.Base;

/// <summary>
///     数据库上下文基础类
/// </summary>
/// <typeparam name="TDbContext">数据库上下文类型</typeparam>
public abstract class BaseDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    #region Extensions

    private Guid? GetUserId()
    {
        var userIdString = HttpContextAccessor?.HttpContext?.User.FindFirst(ClaimKeys.UserId)?.Value;
        return Guid.TryParse(userIdString, out var id) ? id : null;
    }

    #endregion

    #region Constructors

    protected readonly IHttpContextAccessor? HttpContextAccessor;

    /// <summary>
    ///     构造数据库上下文
    /// </summary>
    /// <param name="options">数据库上下文配置</param>
    protected BaseDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }

    /// <summary>
    ///     构造数据库上下文
    /// </summary>
    /// <param name="options">数据库上下文配置</param>
    /// <param name="httpContextAccessor">HTTP上下文访问器</param>
    protected BaseDbContext(DbContextOptions<TDbContext> options, IHttpContextAccessor httpContextAccessor) :
        base(options)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Initialization

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Table prefix

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.ClrType.Name.ToSnakeCase());
        }

        #endregion

        #region Soft deletable filter

        modelBuilder.EntitiesOfType<ISoftDeletable>(builder =>
        {
            var entityType = builder.Metadata.ClrType;
            var param = Expression.Parameter(entityType, "p");
            var body = Expression.Equal(Expression.Property(param, nameof(ISoftDeletable.IsDeleted)),
                Expression.Constant(false));
            builder.HasQueryFilter(Expression.Lambda(body, param));

            var indexAttributes = entityType.GetCustomAttributes<IndexAttribute>();
            foreach (var indexAttribute in indexAttributes)
            {
                var indexNames = indexAttribute.PropertyNames.ToArray();
                var indexBuilder = builder.HasIndex(indexNames)
                    .HasFilter($"{nameof(ISoftDeletable.IsDeleted).ToSnakeCase()} is not true");
                var indexIsUnique = indexAttribute.IsUnique;
                indexBuilder.IsUnique(indexIsUnique);
                var indexIsDescending = indexAttribute.IsDescending;
                if (indexIsDescending is not null)
                {
                    indexBuilder.IsDescending(indexIsDescending);
                }
            }
        });

        #endregion

        #region Force datetime to UTC

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless)
            {
                continue;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        #endregion
    }

    #region Interceptor on saving changes

    #region Saving changes actions

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    #endregion

    /// <summary>
    ///     处理唯一索引
    /// </summary>
    /// <exception cref="DuplicateException">与唯一索引冲突时抛出重复异常</exception>
    private void HandleUniqueIndices()
    {
        var entries = ChangeTracker.Entries();
        var specifyMethod = typeof(DbExtensions).GetMethod(nameof(DbExtensions.GetEntityTupleDictByEntry),
            BindingFlags.Public | BindingFlags.Static)!;
        var validationMethod = typeof(DbExtensions).GetMethod(nameof(DbExtensions.HasDataWithSpecifiedKeys),
            BindingFlags.Public | BindingFlags.Static)!;
        foreach (var entry in entries)
        {
            if (entry.Metadata.IsOwned())
            {
                continue;
            }

            var entity = entry.Entity;
            var clrType = entity.GetType();
            var indices = clrType.GetCustomAttributes<IndexAttribute>(true);
            var uniquePropertyNameGroups = indices.Where(index => index.IsUnique)
                .Select(index => index.PropertyNames).ToArray();
            var typedSpecifyMethod = specifyMethod.MakeGenericMethod(clrType);
            var typedValidationMethod = validationMethod.MakeGenericMethod(clrType);
            var duplicatePropertyName = uniquePropertyNameGroups.FirstOrDefault(propertyNames =>
            {
                var dict = typedSpecifyMethod.Invoke(null, new object[] { entry, propertyNames.ToArray() })!;
                return (bool)typedValidationMethod.Invoke(null, new[] { this, dict, entity })!;
            });
            if (duplicatePropertyName is not null)
            {
                throw new DuplicateException(clrType, duplicatePropertyName.ToArray());
            }
        }
    }

    /// <summary>
    ///     处理审计与软删除内容
    /// </summary>
    private void HandleAuditedOrSoftDeletable()
    {
        var userId = HttpContextAccessor.GetUserId();
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (entry.Entity is IAudited audited)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        audited.CreateBy(userId);
                        break;
                    case EntityState.Modified:
                        audited.ModifyBy(userId);
                        break;
                }
            }

            if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable { IsHardDeleted: false } softDeletable })
            {
                entry.State = EntityState.Modified;
                softDeletable.DeleteBy(userId);
                foreach (var property in entry.Properties)
                {
                    property.IsModified = ISoftDeletable.DbProperties.Contains(property.Metadata.Name);
                }

                foreach (var reference in entry.References)
                {
                    if (reference.TargetEntry?.Metadata.IsOwned() ?? false)
                    {
                        reference.TargetEntry.State = EntityState.Unchanged;
                    }
                }
            }
        }
    }

    #endregion

    #endregion
}