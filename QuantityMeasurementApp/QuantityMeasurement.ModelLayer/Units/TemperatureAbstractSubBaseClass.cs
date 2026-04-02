using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public abstract class Temperature : Quantity
{
    protected Temperature(double value, Unit unit)
        : base(value, unit)
    {
    }

    // --------------------------
    // Convert
    // --------------------------
    public Temperature ConvertTo(Unit targetUnit)
    {
        return (Temperature)base.ConvertTo(targetUnit);
    }

    // --------------------------
    // Add
    // --------------------------
    public Temperature Add(Temperature other)
    {
        return (Temperature)base.Add(other);
    }

    public Temperature Add(Temperature other, Unit targetUnit)
    {
        return (Temperature)base.Add(other, targetUnit);
    }

    // --------------------------
    // Subtract
    // --------------------------
    public Temperature Subtract(Temperature other)
    {
        return (Temperature)base.Subtract(other);
    }

    public Temperature Subtract(Temperature other, Unit targetUnit)
    {
        return (Temperature)base.Subtract(other, targetUnit);
    }

    // --------------------------
    // Divide
    // --------------------------
    public double Divide(Temperature other)
    {
        return base.Divide(other);
    }

    public Temperature Divide(double divisor)
    {
        return (Temperature)base.Divide(divisor);
    }

    // --------------------------
    // Factory Method
    // --------------------------
    public override Quantity CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Celsius) return new Celsius(value);
        if (unit == Unit.Fahrenheit) return new Fahrenheit(value);
        if (unit == Unit.Kelvin) return new Kelvin(value);

        throw new InvalidOperationException("Unsupported temperature unit.");
    }
}