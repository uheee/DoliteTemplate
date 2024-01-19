using AutoMapper;
using DoliteTemplate.Domain.Shared.Utils;

namespace DoliteTemplate.Api.Utils.AutoMapper;

/// <summary>
///     应用映射器
/// </summary>
public class AppProfile : Profile
{
    /// <summary>
    ///     构造应用映射器
    /// </summary>
    public AppProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PageConverter<,>));
    }

    /// <summary>
    ///     分页器内部转换器
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    private class
        PageConverter<TSource, TDestination> : ITypeConverter<PaginatedList<TSource>, PaginatedList<TDestination>>
    {
        public PaginatedList<TDestination> Convert(PaginatedList<TSource> source,
            PaginatedList<TDestination> destination,
            ResolutionContext context)
        {
            var items = context.Mapper.Map<IEnumerable<TDestination>>(source.Content);
            return new PaginatedList<TDestination>(items, source.Count, source.PageIndex, source.PageSize);
        }
    }
}