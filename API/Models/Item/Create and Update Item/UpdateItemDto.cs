namespace WildRiftWebAPI;

public record UpdateItemDto
(
#nullable enable
    string Name,
    int Identifier,
    string Description,
    string Colloq,
    string Plaintext,

    string Recipe,
    string Part_of,

    int Special_recipe,

    string Image_full,
    string Image_sprite,
    string Image_group,
    int Image_x,
    int Image_y,
    int Image_w,
    int Image_h,

    int Gold_based,
    bool Gold_purchasable,
    int Gold_total,
    int Gold_sell,

    string Tags,

    int Stats_flat_hp_pool,
    int Stats_flat_mp_pool,
    int Stats_percent_hp_pool,
    int Stats_percent_mp_pool,
    float Stats_flat_hp_regen,
    int Stats_percent_hp_regen,
    int Stats_flat_mp_regen,
    int Stats_percent_mp_regen,
    int Stats_flat_armor,
    int Stats_percent_armor,
    int Stats_flat_physical_damage,
    int Stats_flat_magic_damage,
    int Stats_flat_movement_speed,
    float Stats_percent_movement_speed,
    float Stats_percent_attack_speed,
    float Stats_flat_crit_chance,
    int Stats_flat_spell_block,
    float Stats_percent_life_steal
#nullable disable
);
