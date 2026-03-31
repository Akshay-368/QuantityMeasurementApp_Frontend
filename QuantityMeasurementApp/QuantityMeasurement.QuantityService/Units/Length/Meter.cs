namespace QuantityMeasurement.QuantityService.Units;
using QuantityMeasurement.QuantityService.Core;
public class Meter : Length
{
    public Meter(double value) : base(value, Unit.Meter)
    {
    }
}