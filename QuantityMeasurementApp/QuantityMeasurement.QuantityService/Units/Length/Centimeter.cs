namespace QuantityMeasurement.QuantityService.Units;
using QuantityMeasurement.QuantityService.Core;
public class Centimeter : Length
{
    public Centimeter(double value) : base(value, Unit.Centimeter)
    {
    }
}