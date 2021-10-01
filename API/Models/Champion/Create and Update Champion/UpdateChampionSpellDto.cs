namespace WildRiftWebAPI
{
    public class UpdateChampionSpellDto
    {
#nullable enable
        public char? Char { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Maxrank { get; set; }
        public string? Cooldown { get; set; }
        public string? Cost { get; set; }
        public string? CostType { get; set; }
        public string? Spell_range { get; set; }
        public string? Resource { get; set; }
#nullable disable
    }
}