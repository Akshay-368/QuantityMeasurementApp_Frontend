using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Kelvin : Temperature
{
    public Kelvin(double value)
        : base(value, Unit.Kelvin)
    {
    }
}