using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    //Enum po to aby okre�li� jak� akcj� chcemy wykona�. Zwi�zane z klas� ResourceOperationRequirement
    public enum ResourceOperation
    {
        Create, Read, Update, Delete
    }
}
