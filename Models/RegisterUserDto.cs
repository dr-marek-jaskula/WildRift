using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WildRiftWebAPI
{
    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int RoleId { get; set; } = 1;
    }
}