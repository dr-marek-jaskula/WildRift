using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

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
