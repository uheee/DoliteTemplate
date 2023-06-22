using AutoMapper;
using DoliteTemplate.Domain.DTOs;
using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Shared.Utils;

namespace DoliteTemplate.Api.Utils.AutoMapper;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PageConverter<,>));
        CreateMap<Device, DeviceReadDto>();
        CreateMap<DeviceCreateDto, Device>();
        CreateMap<DeviceUpdateDto, Device>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
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