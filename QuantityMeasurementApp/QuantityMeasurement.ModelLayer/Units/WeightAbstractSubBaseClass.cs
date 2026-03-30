using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

public abstract class Weight : Quantity
{
    protected Weight(double value, Unit unit) : base(value, unit)
    {
    }

    // --------------------------
    // Convert
    // --------------------------
    public Weight ConvertTo(Unit targetUnit)
    {

        return (Weight)base.ConvertTo(targetUnit);
    }

    // --------------------------
    // Add (default to this unit)
    // --------------------------
    public Weight Add(Weight other)
    {
       return (Weight)base.Add(other);
    }

    // --------------------------
    // Add with target unit
    // --------------------------
    public Weight Add(Weight other, Unit targetUnit)
    {
        

       return (Weight)base.Add(other, targetUnit);
    }

    // --------------------------
    // Factory Method
    // --------------------------
    public override Quantity CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Kilogram) return new Kilogram(value);
        if (unit == Unit.Gram) return new Gram(value);
        if (unit == Unit.Pound) return new Pound(value);

        throw new InvalidOperationException("Unsupported weight unit.");
    }


    public Weight Subtract(Weight other)
    {
        return (Weight)base.Subtract(other);
    }

    public Weight Subtract(Weight other, Unit targetUnit)
    {
        return (Weight)base.Subtract(other, targetUnit);
    }

    public double Divide(Weight other)
    {
        return base.Divide(other);
    }

    public Weight Divide(double divisor)
    {
        return (Weight)base.Divide(divisor);
    }

}