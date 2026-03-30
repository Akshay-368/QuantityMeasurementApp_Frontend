namespace QuantityMeasurement.BusinessLayer.Interfaces;

public interface IAuthService
{
    void Register(string username, string password);
    string Login(string username, string password);
}