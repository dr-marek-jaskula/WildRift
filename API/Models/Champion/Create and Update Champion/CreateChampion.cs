using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public record CreateChampion
    (
        CreateChampionDto CreateChampionDto,
        CreateChampionPassiveDto CreateChampionPassiveDto,
        List<CreateChampionSpellDto> CreateChampionSpellDtos
    );
}