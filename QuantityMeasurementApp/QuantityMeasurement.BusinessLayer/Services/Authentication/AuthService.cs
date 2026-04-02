using QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.ModelLayer.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace QuantityMeasurement.BusinessLayer.Services.Authentication;

public class AuthService : IAuthService
{
    private static readonly Regex UsernameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    private readonly IQuantityDbContext _db;

    private readonly IPasswordHasher _passwordHelper ;

    private readonly IConfiguration _config;

    public AuthService(IQuantityDbContext db , IPasswordHasher passwordHelper , IConfiguration configuration)
    {
        _db = db;
        _passwordHelper = passwordHelper;
        _config = configuration;
    }

    public async Task RegisterAsync(string username, string password)
    {
        var normalizedUsername = ValidateUsername(username, "username");
        ValidateRegistrationPassword(password, "password");

        if (await _db.Users.AnyAsync(u => u.Username == normalizedUsername))
            throw new InvalidOperationException("User already exists.");

        var (hash, salt) = _passwordHelper.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = normalizedUsername,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var normalizedUsername = ValidateUsername(username, "username");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", "password");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == normalizedUsername);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        if (!_passwordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid username or password.");

        return CreateToken(user);
    }

    private static string ValidateUsername(string username, string paramName)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", paramName);

        var normalized = username.Trim();

        if (!UsernameRegex.IsMatch(normalized))
        {
            throw new ArgumentException(
                "Username can contain only letters, numbers, and underscore with no whitespace.",
                paramName
            );
        }

        return normalized;
    }

    private static void ValidateRegistrationPassword(string password, string paramName)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password is required.", paramName);

        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.", paramName);

        if (password.Any(char.IsWhiteSpace))
            throw new ArgumentException("Password cannot contain whitespace.", paramName);

        if (!password.Any(char.IsLetter))
            throw new ArgumentException("Password must include at least one letter.", paramName);

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must include at least one number.", paramName);

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException("Password must include at least one special symbol.", paramName);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = Environment.GetEnvironmentVariable("Jwt__Key")
              ?? throw new Exception("JWT Key missing");

        var issuer = Environment.GetEnvironmentVariable("Jwt__Issuer");
        var audience = Environment.GetEnvironmentVariable("Jwt__Audience");

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        /*
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );
        */

        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}