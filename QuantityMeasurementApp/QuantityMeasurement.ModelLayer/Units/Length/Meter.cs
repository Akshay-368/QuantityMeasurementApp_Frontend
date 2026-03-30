namespace QuantityMeasurement.ModelLayer.Units;
using QuantityMeasurement.ModelLayer.Core;
public class Meter : Length
{
    public Meter(double value) : base(value, Unit.Meter)
    {
    }
}