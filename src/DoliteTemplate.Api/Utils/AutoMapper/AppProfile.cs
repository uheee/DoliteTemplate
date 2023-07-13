using AutoMapper;
using DoliteTemplate.Shared.Utils;

namespace DoliteTemplate.Api.Utils.AutoMapper;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PageConverter<,>));
        // Set custom mappers
    }

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