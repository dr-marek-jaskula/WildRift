using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User User { get; set; }
    }
}
