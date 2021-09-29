using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WildRiftWebAPI
{
    public class RuneMappingProfile : Profile
    {
        public RuneMappingProfile()
        {
            CreateMap<Rune, RuneDto>();
            CreateMap<UpdateRuneDto, Rune>();
            CreateMap<CreateRuneDto, Rune>();
        }
    }
}
