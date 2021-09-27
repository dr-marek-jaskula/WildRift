using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class CreateChampionPassiveDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image_full { get; set; }
        public string Image_sprite { get; set; }
        public string Image_group { get; set; }
        public int Image_x { get; set; }
        public int Image_y { get; set; }
        public int Image_w { get; set; }
        public int Image_h { get; set; }
    }
}
