using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    //Enum po to aby okreœliæ jak¹ akcjê chcemy wykonaæ. Zwi¹zane z klas¹ ResourceOperationRequirement
    public enum ResourceOperation
    {
        Create, Read, Update, Delete
    }
}
