namespace WildRiftWebAPI
{
    public record UpdateChampionSpellDto
    (
#nullable enable
        char? Char,
        string? Name,
        string? Description,
        int? Maxrank,
        string? Cooldown,
        string? Cost,
        string? CostType,
        string? Spell_range,
        string? Resource
#nullable disable
    );
}