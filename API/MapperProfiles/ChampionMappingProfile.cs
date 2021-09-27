using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WildRiftWebAPI
{
    public class ChampionMappingProfile : Profile
    {
        public ChampionMappingProfile()
        {
            CreateMap<Champion, ChampionDto>();
            CreateMap<ChampionSpell, ChampionSpellDto>();
            CreateMap<ChampionPassive, ChampionPassiveDto>();

            CreateMap<Item, ItemDto>();
            CreateMap<UpdateItemDto, Item>();
            CreateMap<CreateItemDto, Item>();

            CreateMap<CreateChampionDto, Champion>()
                .ForMember(ch => ch.Spell_passive, c => c.MapFrom(dto => $"{dto.Name}Passive"))
                .ForMember(ch => ch.Spell_q, c => c.MapFrom(dto => $"{dto.Name}Q"))
                .ForMember(ch => ch.Spell_w, c => c.MapFrom(dto => $"{dto.Name}W"))
                .ForMember(ch => ch.Spell_e, c => c.MapFrom(dto => $"{dto.Name}E"))
                .ForMember(ch => ch.Spell_r, c => c.MapFrom(dto => $"{dto.Name}R"));

            CreateMap<CreateChampionPassiveDto, ChampionPassive>()
                .ForMember(ch => ch.Champion, c => c.MapFrom(dto => dto.Id.Substring(0, dto.Id.Length - "Passive".Length)));

            CreateMap<CreateChampionSpellDto, ChampionSpell>()
                .ForMember(ch => ch.Champion, c => c.MapFrom(dto => dto.Id.Substring(0, dto.Id.Length - "Q".Length)));

            CreateMap<UpdateChampionDto, Champion>();
            CreateMap<UpdateChampionPassiveDto, ChampionPassive>();
            CreateMap<UpdateChampionSpellDto, ChampionSpell>();
        }
    }
}
