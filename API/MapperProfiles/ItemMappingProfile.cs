using AutoMapper;

namespace WildRiftWebAPI
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<UpdateItemDto, Item>();
            CreateMap<CreateItemDto, Item>();
        }
    }
}