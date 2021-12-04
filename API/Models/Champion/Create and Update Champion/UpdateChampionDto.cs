namespace WildRiftWebAPI;

public record UpdateChampionDto
(
#nullable enable
    string? Title,
    string? Lore,
    bool? Available,
    int? Difficulty,
    string? Tier,
    int? Damage,
    int? Utility,
    int? Toughness,
    string? Roles,
    float? Stats_hp,
    float? Stats_hpperlevel,
    float? Stats_mp,
    float? Stats_mpperlevel,
    float? Stats_movespeed,
    float? Stats_armor,
    float? Stats_armorperlevel,
    float? Stats_spellblock,
    float? Stats_spellblockperlevel,
    float? Stats_attackrange,
    float? Stats_hpregen,
    float? Stats_hpregenperlevel,
    float? Stats_mpregen,
    float? Stats_mpregenperlevel,
    float? Stats_crit,
    float? Stats_critperlevel,
    float? Stats_attackdamage,
    float? Stats_attackdamageperlevel,
    float? Stats_attackspeed,
    float? Stats_attackspeedperlevel
#nullable disable
);
