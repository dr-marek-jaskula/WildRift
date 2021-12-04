namespace WildRiftWebAPI;

public class Item : IName
{
    public string Name { get; set; }
    public int Identifier { get; set; }
    public string Description { get; set; }
    public string Colloq { get; set; }
    public string Plaintext { get; set; }

    public string Recipe { get; set; }
    public string Part_of { get; set; }

    public int Special_recipe { get; set; }

    public string Image_full { get; set; }
    public string Image_sprite { get; set; }
    public string Image_group { get; set; }
    public int Image_x { get; set; }
    public int Image_y { get; set; }
    public int Image_w { get; set; }
    public int Image_h { get; set; }

    public int Gold_based { get; set; }
    public bool Gold_purchasable { get; set; }
    public int Gold_total { get; set; }
    public int Gold_sell { get; set; }

    public string Tags { get; set; }

    public int Stats_flat_hp_pool { get; set; }
    public int Stats_flat_mp_pool { get; set; }
    public int Stats_percent_hp_pool { get; set; }
    public int Stats_percent_mp_pool { get; set; }
    public float Stats_flat_hp_regen { get; set; }
    public int Stats_percent_hp_regen { get; set; }
    public int Stats_flat_mp_regen { get; set; }
    public int Stats_percent_mp_regen { get; set; }
    public int Stats_flat_armor { get; set; }
    public int Stats_percent_armor { get; set; }
    public int Stats_flat_physical_damage { get; set; }
    public int Stats_flat_magic_damage { get; set; }
    public int Stats_flat_movement_speed { get; set; }
    public float Stats_percent_movement_speed { get; set; }
    public float Stats_percent_attack_speed { get; set; }
    public float Stats_flat_crit_chance { get; set; }
    public int Stats_flat_spell_block { get; set; }
    public float Stats_percent_life_steal { get; set; }
}
