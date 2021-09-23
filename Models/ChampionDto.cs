using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class ChampionDto
    {
        public Champion Champion { get; set; }
        public List<ChampionSpell> ChampionSpells { get; set; }
        public ChampionPassive ChampionPassive { get; set; }
    }
}
