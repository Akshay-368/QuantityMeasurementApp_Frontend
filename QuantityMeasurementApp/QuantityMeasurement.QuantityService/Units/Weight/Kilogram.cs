using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Kilogram : Weight
{
    public Kilogram(double value) : base(value, Unit.Kilogram)
    {
    }
}