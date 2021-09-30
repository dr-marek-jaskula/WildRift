using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public record RuneDto(string Name, string Icon, string ShortDesc, string LongDesc, string Path, string Slot);
}
