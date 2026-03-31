using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Fahrenheit : Temperature
{
    public Fahrenheit(double value)
        : base(value, Unit.Fahrenheit)
    {
    }
}