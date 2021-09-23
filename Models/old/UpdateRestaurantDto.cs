using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WildRiftWebAPI
{
    public class UpdateRestaurantDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasDelivery { get; set; }
    }
}
