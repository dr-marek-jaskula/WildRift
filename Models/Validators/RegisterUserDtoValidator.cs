using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WildRiftWebAPI
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password); 

            RuleFor(x => x.Email).Custom((value, contex) =>
            {
                bool emialInUse = dbContext.Users.Any(u => u.Email == value);
                if (emialInUse) contex.AddFailure("Email", "That email is taken");
            });
        }

    }
}
