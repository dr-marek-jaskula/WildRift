namespace WildRiftWebAPI
{
    public record ChampionSpellDto
    (
        string Id,
        string Name,
        string Description,
        int Maxrank,
        string Cooldown,
        string Cost,
        string CostType,
        string Spell_range,
        string Image_full,
        string Image_sprite,
        string Image_group,
        int Image_x,
        int Image_y,
        int Image_w,
        int Image_h,
        string Resource,
        string Champion
    );
}