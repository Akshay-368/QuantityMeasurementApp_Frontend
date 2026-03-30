using QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.ModelLayer.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace QuantityMeasurement.BusinessLayer.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly IQuantityDbContext _db;

    private readonly IPasswordHasher _passwordHelper ;

    private readonly IConfiguration _config;

    public AuthService(IQuantityDbContext db , IPasswordHasher passwordHelper , IConfiguration configuration)
    {
        _db = db;
        _passwordHelper = passwordHelper;
        _config = configuration;
    }

    public void Register(string username, string password)
    {
        if ( _db.Users.Any(u => u.Username == username)) throw new Exception("User already exists");
        var (hash, salt) = _passwordHelper.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public string Login(string username, string password)
    {
        // if (_db.Users.Any(u => u.Username == username)) throw new Exception("User already exists");
        // wrong here as during login user must exists . So this check should be removed and used in registratin instead as during that user must not exists 
        var user = _db.Users.FirstOrDefault(u => u.Username == username);

        if (user == null)
            throw new Exception("User not found");

        if (!_passwordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            throw new Exception("Invalid password");

        return CreateToken(user);
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