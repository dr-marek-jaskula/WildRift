using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public record UpdateChampion
    (
        UpdateChampionDto UpdateChampionDto,
        UpdateChampionPassiveDto UpdateChampionPassiveDto,
        List<UpdateChampionSpellDto> UpdateChampionSpellDtos
    );
}