namespace QuantityMeasurement.QuantityService.Units;
using QuantityMeasurement.QuantityService.Core;
public class Yard : Length
{
    public Yard(double value) : base(value, Unit.Yard)
    {
    }
}