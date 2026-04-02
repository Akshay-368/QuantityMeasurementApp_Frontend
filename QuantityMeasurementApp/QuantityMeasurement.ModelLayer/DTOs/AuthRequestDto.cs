namespace QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.ModelLayer.Interfaces;
public class AuthRequestDto : IAuthRequestDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}