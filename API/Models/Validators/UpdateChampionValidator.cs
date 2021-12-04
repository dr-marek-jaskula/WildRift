using FluentValidation;
namespace WildRiftWebAPI;

public class UpdateChampionValidator : AbstractValidator<UpdateChampion>
{
    public UpdateChampionValidator()
    {
        RuleForEach(x => x.UpdateChampionSpellDtos).ChildRules(spell => spell.RuleFor(x => x.Char).Custom((value, contex) => { if (value is not null && !(new[] { 'Q', 'W', 'E', 'R' }).Contains((char)value)) contex.AddFailure("Char", "Char has to be null or \'Q\', \'W\', \'E\', \'R\'"); }));
    }
}
