namespace QuantityMeasurement.BusinessLayer.Services.Security;
using QuantityMeasurement.BusinessLayer.Interfaces;
using System.Text;
using System.Security.Cryptography;


public  class PasswordHasher : IPasswordHasher
{
    public  (byte[] hash, byte[] salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA256(); // gives salt automatically as compared to raw SH256

        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return (hash, salt);
    }

    public  bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA256(storedSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(storedHash);
    }
}