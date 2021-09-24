using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class UpdateChampionDto
    {
#nullable enable
        public string? Title { get; set; }
        public float? Stats_hp { get; set; }
        public float? Stats_hpperlevel { get; set; }
        public float? Stats_mp { get; set; }
        public float? Stats_mpperlevel { get; set; }
        public float? Stats_movespeed { get; set; }
        public float? Stats_armor { get; set; }
        public float? Stats_armorperlevel { get; set; }
        public float? Stats_spellblock { get; set; }
        public float? Stats_spellblockperlevel { get; set; }
        public float? Stats_attackrange { get; set; }
        public float? Stats_hpregen { get; set; }
        public float? Stats_hpregenperlevel { get; set; }
        public float? Stats_mpregen { get; set; }
        public float? Stats_mpregenperlevel { get; set; }
        public float? Stats_crit { get; set; }
        public float? Stats_critperlevel { get; set; }
        public float? Stats_attackdamage { get; set; }
        public float? Stats_attackdamageperlevel { get; set; }
        public float? Stats_attackspeed { get; set; }
        public float? Stats_attackspeedperlevel { get; set; }
#nullable disable
    }
}
