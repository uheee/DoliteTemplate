using AutoMapper;

namespace DoliteTemplate.Api.Utils;

public static class AutoMapperExtensions
{
    public static void OnlyForNotNullProperties<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        expression.ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }

    public static void CreateCrudMap<TEntity, TReadDto, TCreateDto, TUpdateDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TReadDto>>? read = null,
        Action<IMappingExpression<TCreateDto, TEntity>>? create = null,
        Action<IMappingExpression<TUpdateDto, TEntity>>? update = null)
    {
        var readExpression = profile.CreateMap<TEntity, TReadDto>();
        read?.Invoke(readExpression);
        var createExpression = profile.CreateMap<TCreateDto, TEntity>();
        create?.Invoke(createExpression);
        var updateExpression = profile.CreateMap<TUpdateDto, TEntity>();
        updateExpression.OnlyForNotNullProperties();
        update?.Invoke(updateExpression);
    }

    public static void CreateCrudMap<TEntity, TReadDto, TCreateUpdateDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TReadDto>>? read = null,
        Action<IMappingExpression<TCreateUpdateDto, TEntity>>? create = null,
        Action<IMappingExpression<TCreateUpdateDto, TEntity>>? update = null)
    {
        CreateCrudMap<TEntity, TReadDto, TCreateUpdateDto, TCreateUpdateDto>(profile, read, create, update);
    }

    public static void CreateCrudMap<TEntity, TDto>(this Profile profile,
        Action<IMappingExpression<TEntity, TDto>>? read = null,
        Action<IMappingExpression<TDto, TEntity>>? create = null,
        Action<IMappingExpression<TDto, TEntity>>? update = null)
    {
        CreateCrudMap<TEntity, TDto, TDto, TDto>(profile, read, create, update);
    }
}