using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    /// <summary>
    /// Its a class for clients 
    /// </summary>
    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}