using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Celsius : Temperature
{
    public Celsius(double value)
        : base(value, Unit.Celsius)
    {
    }
}