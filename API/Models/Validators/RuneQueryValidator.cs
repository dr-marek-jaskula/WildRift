using FluentValidation;
namespace WildRiftWebAPI;

public class RuneQueryValidator : AbstractValidator<RuneQuery>
{
    private readonly int[] _allowedPageSizes = new[] { 5, 10, 15 };
    private readonly string[] _allowedSortByColumnNames = { nameof(RuneDto.Name), nameof(RuneDto.Path), nameof(Rune.Slot) };

    public RuneQueryValidator()
    {
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize).Custom((value, context) =>
        {
            if (!_allowedPageSizes.Contains(value))
                context.AddFailure("PageSize", $"PageSize must be in [{string.Join(",", _allowedPageSizes)}]");
        });

        RuleFor(r => r.SortBy)
            .Must(value => string.IsNullOrEmpty(value) || _allowedSortByColumnNames.Contains(value))
            .WithMessage($"Sort by is optional or must be in [{string.Join(",", _allowedSortByColumnNames)}]");
    }
}
