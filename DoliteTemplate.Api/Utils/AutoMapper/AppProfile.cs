using AutoMapper;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Domain.Utils;

namespace DoliteTemplate.Api.Utils.AutoMapper;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PageConverter<,>));
        CreateMap<Device, DeviceReadDto>();
        CreateMap<DeviceCreateDto, Device>();
        CreateMap<DeviceUpdateDto, Device>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
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