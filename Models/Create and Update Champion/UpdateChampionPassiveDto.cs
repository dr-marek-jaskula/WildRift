﻿using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class UpdateChampionPassiveDto
    {
#nullable enable
        public string? Name { get; set; }
        public string? Description { get; set; }
#nullable disable
    }
}
