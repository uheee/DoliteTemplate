using AutoMapper;

namespace DoliteTemplate.Api.Utils;

public static class AutoMapperExtensions
{
    public static void OnlyForNotNullProperties<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        expression.ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}