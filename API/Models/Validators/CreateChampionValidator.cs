using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WildRiftWebAPI
{
    public class CreateChampionValidator : AbstractValidator<CreateChampion>
    {
        public CreateChampionValidator()
        {
        }
    }
}
