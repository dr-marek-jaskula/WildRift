using AutoMapper;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class ChampionMappingProfile : Profile
    {
        public ChampionMappingProfile()
        {
            CreateMap<Champion, ChampionDto>();
            CreateMap<ChampionSpell, ChampionSpellDto>();
            CreateMap<ChampionPassive, ChampionPassiveDto>();
        }
    }
}
