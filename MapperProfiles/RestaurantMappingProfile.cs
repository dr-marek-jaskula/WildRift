using AutoMapper;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(m => m.City, dto => dto.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, dto => dto.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, dto => dto.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>(); //jeœli typy i nazwy w³aœciwoœci siê zgadzaj¹, to AutoMapper zmapuje automatycznie.

            //ten profil mappowania robi z trzech propsów obiekt, gdzie te propsy nale¿¹
            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(r => r.Address, c => c.MapFrom(dto => new Address() { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street }));
        }
    }
}
