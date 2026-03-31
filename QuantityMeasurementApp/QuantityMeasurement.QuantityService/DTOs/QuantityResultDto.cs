namespace QuantityMeasurement.QuantityService.DTOs;
using QuantityMeasurement.QuantityService.Interfaces;
public class QuantityResultDto : IQuantityResultDto
{
    public double Result { get; set; }
    public string Unit { get ; set ; } = string.Empty;
}