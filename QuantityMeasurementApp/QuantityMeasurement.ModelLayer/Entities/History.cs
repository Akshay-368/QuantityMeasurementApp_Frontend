namespace QuantityMeasurement.ModelLayer.Entities;

public class History
{
    public Guid Id { get; set; } // Globally Unique Identifier

    public string Operation { get; set; } = string.Empty;

    public double Value1 { get; set; }
    public string Unit1 { get; set; } = string.Empty;

    public double? Value2 { get; set; }
    public string? Unit2 { get; set; }

    public string? TargetUnit { get; set; }

    public double? Scalar { get; set; }

    public double Result { get; set; }

    public string ResultUnit { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}