using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Pound : Weight
{
    public Pound(double value) : base(value, Unit.Pound)
    {
    }
}