namespace QuantityMeasurement.ModelLayer.Interfaces;
public interface IQuantityRequestDto
{
    double Value1 { get; set; }
    string Unit1 { get; set; }

    double? Value2 { get; set; }
    string? Unit2 { get; set; }

    string? TargetUnit { get; set; }

    double? Scalar { get; set; }
}