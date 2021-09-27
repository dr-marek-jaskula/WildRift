using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class UpdateChampion
    {
        public UpdateChampionDto UpdateChampionDto { get; set; }
        public UpdateChampionPassiveDto UpdateChampionPassiveDto { get; set; }
        public List<UpdateChampionSpellDto> UpdateChampionSpellDtos { get; set; }
    }
}
