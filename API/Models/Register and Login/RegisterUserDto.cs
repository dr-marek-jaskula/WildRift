using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WildRiftWebAPI
{
    public record RegisterUserDto (string Username, string Email, string Password, string ConfirmPassword, int RoleId = 1);
}