namespace QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.ModelLayer.Interfaces;
public class QuantityResultDto : IQuantityResultDto
{
    public double Result { get; set; }
    public string Unit { get ; set ; } = string.Empty;
}