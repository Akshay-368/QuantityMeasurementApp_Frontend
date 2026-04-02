namespace QuantityMeasurement.ModelLayer.Interfaces;

public interface IUnit
{
    string Name { get; }
    double ConversionFactorToBase { get; }
    double OffsetToBase { get; }
    string Category { get; }

    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double value);
}