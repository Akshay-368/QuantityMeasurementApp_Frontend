using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Kilogram : Weight
{
    public Kilogram(double value) : base(value, Unit.Kilogram)
    {
    }
}