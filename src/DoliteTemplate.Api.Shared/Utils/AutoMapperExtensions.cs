using AutoMapper;
using DoliteTemplate.Api.Shared.Services.Base;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     Auto Mapper扩展
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    ///     配置两个实体的映射关系仅对非null属性生效
    ///     <remarks>一般适用于Update -> Entity且需要忽略空属性的情况</remarks>
    /// </summary>
    /// <param name="expression">映射表达式</param>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    public static void OnlyForNotNullProperties<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        expression.ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }

    /// <summary>
    ///     配置可空类型映射
    ///     <remarks>适用于值类型可空映射情况</remarks>
    /// </summary>
    /// <param name="profile">Auto Mapper的Profile</param>
    /// <typeparam name="TValueType">值类型</typeparam>
    public static void CreateNullableMap<TValueType>(this Profile profile) where TValueType : struct
    {
        profile.CreateMap<TValueType?, TValueType>().ConvertUsing((src, dst) => src ?? dst);
    }

    /// <summary>
    ///     配置增删改查
    /// </summary>
    /// <param name="profile">Auto Mapper的Profile</param>
    /// <param name="overall">Entity -> Overall映射表达式</param>
    /// <param name="detail">Entity -> Detail映射表达式</param>
    /// <param name="create">Create -> Entity映射表达式</param>
    /// <param name="update">Update -> Entity映射表达式</param>
    /// <inheritdoc cref="IEntityCrudService{TEntity,TOverallDto,TDetailDto,TCreateDto,TUpdateDto}" />
    public static void CreateCrudMap<TEntity, TOverallDto, TDetailDto, TCreateDto, TUpdateDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TOverallDto>>? overall = null,
        Action<IMappingExpression<TEntity, TDetailDto>>? detail = null,
        Action<IMappingExpression<TCreateDto, TEntity>>? create = null,
        Action<IMappingExpression<TUpdateDto, TEntity>>? update = null)
    {
        var overallExpression = profile.CreateMap<TEntity, TOverallDto>();
        overall?.Invoke(overallExpression);
        var detailExpression = profile.CreateMap<TEntity, TDetailDto>();
        detail?.Invoke(detailExpression);
        var createExpression = profile.CreateMap<TCreateDto, TEntity>();
        create?.Invoke(createExpression);
        var updateExpression = profile.CreateMap<TUpdateDto, TEntity>();
        updateExpression.OnlyForNotNullProperties();
        update?.Invoke(updateExpression);
    }

    /// <summary>
    ///     配置增删改查
    /// </summary>
    /// <param name="profile">Auto Mapper的Profile</param>
    /// <param name="read">Entity -> Read映射表达式</param>
    /// <param name="create">Create -> Entity映射表达式</param>
    /// <param name="update">Update -> Entity映射表达式</param>
    /// <inheritdoc cref="IEntityCrudService{TEntity,TReadDto,TCreateDto,TUpdateDto}" />
    public static void CreateCrudMap<TEntity, TReadDto, TCreateDto, TUpdateDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TReadDto>>? read = null,
        Action<IMappingExpression<TCreateDto, TEntity>>? create = null,
        Action<IMappingExpression<TUpdateDto, TEntity>>? update = null)
    {
        CreateCrudMap<TEntity, TReadDto, TReadDto, TCreateDto, TUpdateDto>(profile, null, read, create, update);
    }

    /// <summary>
    ///     配置增删改查
    /// </summary>
    /// <param name="profile">Auto Mapper的Profile</param>
    /// <param name="read">Entity -> Read映射表达式</param>
    /// <param name="write">CreateUpdate -> Entity映射表达式</param>
    /// <inheritdoc cref="IEntityCrudService{TEntity,TReadDto,TCreateUpdateDto}" />
    public static void CreateCrudMap<TEntity, TReadDto, TWriteDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TReadDto>>? read = null,
        Action<IMappingExpression<TWriteDto, TEntity>>? write = null)
    {
        CreateCrudMap<TEntity, TReadDto, TWriteDto, TWriteDto>(profile, read, write);
    }
}

/// <summary>
///     占位用空类
/// </summary>
public class EmptyClass
{
}