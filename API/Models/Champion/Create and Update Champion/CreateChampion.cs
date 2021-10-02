using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class CreateChampion
    {
        public CreateChampionDto CreateChampionDto { get; set; }
        public CreateChampionPassiveDto CreateChampionPassiveDto { get; set; }
        public List<CreateChampionSpellDto> CreateChampionSpellDtos { get; set; }
    }
}