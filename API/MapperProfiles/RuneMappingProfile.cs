using AutoMapper;

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