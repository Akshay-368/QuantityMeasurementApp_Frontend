namespace QuantityMeasurement.BusinessLayer.Interfaces;

public interface IPasswordHasher
{
    (byte[] hash, byte[] salt) HashPassword(string password);
    bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
}