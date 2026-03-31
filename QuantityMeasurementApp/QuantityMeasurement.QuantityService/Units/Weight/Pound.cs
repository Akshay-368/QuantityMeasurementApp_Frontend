using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Pound : Weight
{
    public Pound(double value) : base(value, Unit.Pound)
    {
    }
}