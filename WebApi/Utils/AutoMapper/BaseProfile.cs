using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Utils;

namespace WebApi.Utils.AutoMapper;

public class BaseProfile : Profile
{
    public BaseProfile()
    {
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PageConverter<,>));
        CreateMap<Device, DeviceReadDto>();
        // Set custom mappers
    }

    private class PageConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination,
            ResolutionContext context)
        {
            var items = context.Mapper.Map<IEnumerable<TDestination>>(source.Content);
            return new PagedList<TDestination>(items, source.ItemCount, source.Index, source.PageSize);
        }
    }
}