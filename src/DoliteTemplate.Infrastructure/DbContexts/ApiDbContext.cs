using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Infrastructure.Utils;
using DoliteTemplate.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace DoliteTemplate.Infrastructure.DbContexts;

public class ApiDbContext : DbContext
{
    public IHttpContextAccessor? HttpContextAccessor { get; init; }

    #region Entities

    // Add DbSets here
    public DbSet<Order> Orders { get; init; } = null!;

    #endregion

    #region Constructors

    private readonly DbConnection? _connection;
    private readonly string? _connectionString;

    public ApiDbContext(DbConnection connection)
    {
        _connection = connection;
    }

    public ApiDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    [ActivatorUtilitiesConstructor]
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    #endregion

    #region Initialization

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (builder.IsConfigured)
        {
            return;
        }

        if (_connection is not null)
        {
            builder.UseNpgsql(_connection);
        }
        else if (_connectionString is not null)
        {
            builder.UseNpgsql(_connectionString);
        }

        // builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Table prefix

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName($"T_{entityType.ClrType.Name}");
        }

        modelBuilder.EntitiesOfType<BaseRecord>(builder => builder
            .ToTable($"P_{builder.Metadata.ClrType.Name}", tableBuilder => tableBuilder.ExcludeFromMigrations()));

        #endregion

        #region Soft deletable filter

        modelBuilder.EntitiesOfType<ISoftDeletable>(builder =>
        {
            var entityType = builder.Metadata.ClrType;
            var param = Expression.Parameter(entityType, "p");
            var body = Expression.Equal(Expression.Property(param, nameof(ISoftDeletable.IsDeleted)),
                Expression.Constant(false));
            builder.HasQueryFilter(Expression.Lambda(body, param));

            foreach (var indexAttribute in entityType.GetCustomAttributes<IndexAttribute>())
            {
                var indexNames = indexAttribute.PropertyNames.ToArray();
                var indexBuilder = builder.HasIndex(indexNames)
                    .HasFilter($@"""{nameof(ISoftDeletable.IsDeleted)}"" is not true");
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

        #region Entity configurations

        // Add entity configurations here

        #endregion
    }

    #region Interceptor on saving changes

    #region Saving changes actions

    public override int SaveChanges()
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        HandleUniqueIndices();
        HandleAuditedOrSoftDeletable();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    #endregion

    private void HandleUniqueIndices()
    {
        var entries = ChangeTracker.Entries();
        var specifyMethod = typeof(DbExtensions).GetMethod(nameof(DbExtensions.GetEntityTupleDictByEntry),
            BindingFlags.Public | BindingFlags.Static)!;
        var validationMethod = typeof(DbExtensions).GetMethod(nameof(DbExtensions.HasDataWithSpecifiedKeys),
            BindingFlags.Public | BindingFlags.Static)!;
        foreach (var entry in entries)
        {
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

    private void HandleAuditedOrSoftDeletable()
    {
        var userId = GetUserId();
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
                    property.IsModified = property.Metadata.Name == nameof(ISoftDeletable.IsDeleted);
                }
            }
        }
    }

    private Guid? GetUserId()
    {
        var userIdString = HttpContextAccessor?.HttpContext?.User.FindFirst(ClaimKeys.UserId)?.Value;
        return Guid.TryParse(userIdString, out var id) ? id : null;
    }

    #endregion

    #endregion
}