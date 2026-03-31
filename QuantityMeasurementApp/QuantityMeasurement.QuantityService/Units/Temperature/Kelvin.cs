using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Kelvin : Temperature
{
    public Kelvin(double value)
        : base(value, Unit.Kelvin)
    {
    }
}