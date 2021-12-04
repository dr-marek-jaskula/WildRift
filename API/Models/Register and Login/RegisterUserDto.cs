namespace WildRiftWebAPI;

public record RegisterUserDto(string Username, string Email, string Password, string ConfirmPassword, int RoleId = 1);
