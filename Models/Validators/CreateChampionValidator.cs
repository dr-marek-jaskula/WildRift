using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WildRiftWebAPI
{
    //i zarejestrowaæ
    public class CreateChampionValidator : AbstractValidator<UpdateChampion>
    {
        private readonly int[] allowedPageSizes = new[] {5, 10, 15};
        private readonly string[] allowedSortByColumnNames = { nameof(ChampionDto.Name), nameof(ChampionDto.Title) };

        public CreateChampionValidator()
        {

        }
    }
}
