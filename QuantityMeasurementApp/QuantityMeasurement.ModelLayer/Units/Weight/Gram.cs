using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public class Gram : Weight
{
    public Gram(double value) : base(value, Unit.Gram)
    {
    }
}