namespace QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.ModelLayer.Interfaces;
public class QuantityRequestDto : IQuantityRequestDto
{
    public double Value1 { get; set; }
    public string Unit1 { get; set; } = string.Empty;

    public double? Value2 { get; set; }
    public string? Unit2 { get; set; }

    public string? TargetUnit { get; set; }

    public double? Scalar { get; set; }

}
