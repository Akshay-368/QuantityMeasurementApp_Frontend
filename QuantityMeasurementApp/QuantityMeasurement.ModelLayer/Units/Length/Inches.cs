using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

/// <summary>
/// This is a class for the Inches unit  that inherits from the  Quantity class.
/// It has a constructor that takes a double value and passes it along with the Unit.Inch value to the base constructor of the Quantity class
/// </summary>
public class Inches : Length
{
    // Just like Feet, but I pass Unit.Inch to the base constructor
    public Inches(double value) : base(value, Unit.Inch)
    {
    }
}