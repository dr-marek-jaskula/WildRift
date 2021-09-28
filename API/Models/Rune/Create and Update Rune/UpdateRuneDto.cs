﻿using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class UpdateRuneDto
    {
#nullable enable
        public string? Icon { get; set; }
        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; }
        public string? Path { get; set; }
        public string? Slot { get; set; }
#nullable disable
    }
}