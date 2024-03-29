using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace WildRiftWebAPI;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);

    string GenerateJwt(LoginDto dto);
}

public class AccountService : IAccountService
{
    private readonly WildRiftDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountService(WildRiftDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        var newUser = new User()
        {
            Email = dto.Email,
            Role_id = dto.RoleId,
            Username = dto.Username,
            Create_time = DateTime.Now
        };
        string hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.Password_hash = hashedPassword;

        _context.Users.Add(newUser);
        _context.SaveChanges();
    }

    public string GenerateJwt(LoginDto dto)
    {
        var user = _context.Users
            .Include(user => user.Role)
            .FirstOrDefault(user => user.Username == dto.Username);

        if (user is null)
            throw new BadRequestException("Invalid user name or password");

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password_hash, dto.Password);

        if (result is PasswordVerificationResult.Failed)
            throw new BadRequestException("Invalid username or password");

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            new Claim("Create_time", user.Create_time.Value.ToString("yyyy-MM-dd")),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims: claims, expires: expires, signingCredentials: cred);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}
