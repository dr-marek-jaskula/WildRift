using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WildRiftWebAPI
{
    public class RuneQueryValidator : AbstractValidator<RuneQuery>
    {
        private readonly int[] allowedPageSizes = new[] {5, 10, 15};
        private readonly string[] allowedSortByColumnNames = { nameof(RuneDto.Name), nameof(RuneDto.Path), nameof(Rune.Slot) };

        public RuneQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                    context.AddFailure("PageSize", $"PageSize must be in [{string.Join(",", allowedPageSizes)}]");
            });

            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }
    }
}
