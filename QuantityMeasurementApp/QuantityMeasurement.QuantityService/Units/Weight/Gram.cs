using QuantityMeasurement.QuantityService.Core;

namespace QuantityMeasurement.QuantityService.Units;

public class Gram : Weight
{
    public Gram(double value) : base(value, Unit.Gram)
    {
    }
}