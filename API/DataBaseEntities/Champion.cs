using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class Champion
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Image_full { get; set; }
        public string Image_sprite { get; set; }
        public string Image_group { get; set; }
        public int Image_x { get; set; }
        public int Image_y { get; set; }
        public int Image_w { get; set; }
        public int Image_h { get; set; }
        public float Stats_hp { get; set; }
        public float Stats_hpperlevel { get; set; }
        public float Stats_mp { get; set; }
        public float Stats_mpperlevel { get; set; }
        public float Stats_movespeed { get; set; }
        public float Stats_armor { get; set; }
        public float Stats_armorperlevel { get; set; }
        public float Stats_spellblock { get; set; }
        public float Stats_spellblockperlevel { get; set; }
        public float Stats_attackrange { get; set; }
        public float Stats_hpregen { get; set; }
        public float Stats_hpregenperlevel { get; set; }
        public float Stats_mpregen { get; set; }
        public float Stats_mpregenperlevel { get; set; }
        public float Stats_crit { get; set; }
        public float Stats_critperlevel { get; set; }
        public float Stats_attackdamage { get; set; }
        public float Stats_attackdamageperlevel { get; set; }
        public float Stats_attackspeed { get; set; }
        public float Stats_attackspeedperlevel { get; set; }

        public string Spell_passive { get; set; }
        public string Spell_q { get; set; }
        public string Spell_w { get; set; }
        public string Spell_e { get; set; }
        public string Spell_r { get; set; }

        public virtual ChampionPassive ChampionPassive { get; set; }
        public virtual List<ChampionSpell> ChampionSpells { get; set; }
    }
}
