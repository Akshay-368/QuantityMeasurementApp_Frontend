using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Fahrenheit : Temperature
{
    public Fahrenheit(double value)
        : base(value, Unit.Fahrenheit)
    {
    }
}