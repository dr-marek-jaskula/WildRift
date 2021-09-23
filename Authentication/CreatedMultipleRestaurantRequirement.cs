using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class CreatedMultipleRestaurantRequirement : IAuthorizationRequirement
    {
        public int MinimumRestaurantCreated { get; set; }

        public CreatedMultipleRestaurantRequirement(int minimumRestaurantCreated)
        {
            MinimumRestaurantCreated = minimumRestaurantCreated;
        }
    }
}
