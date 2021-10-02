namespace WildRiftWebAPI
{
    public class Rune : IName
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string Path { get; set; }
        public string Slot { get; set; }
    }
}