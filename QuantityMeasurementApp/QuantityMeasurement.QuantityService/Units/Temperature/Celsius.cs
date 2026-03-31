using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Celsius : Temperature
{
    public Celsius(double value)
        : base(value, Unit.Celsius)
    {
    }
}